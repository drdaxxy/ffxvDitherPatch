﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ffxvDitherPatch
{
    class Patcher
    {
        private Craf _archive;

        // Example:
        //    0x00000424: mul r0.x, r0.x, l(16.000000)    38 00 00 07 | 12 00 10 00 | 00 00 00 00 | 0A 00 10 00 | 00 00 00 00 | 01 40 00 00 | 00 00 80 41
        //    0x00000440: round_ne r0.x, r0.x             40 00 00 05 | 12 00 10 00 | 00 00 00 00 | 0A 00 10 00 | 00 00 00 00
        //    0x00000454: ftou r0.x, r0.x                 1C 00 00 05 | 12 00 10 00 | 00 00 00 00 | 0A 00 10 00 | 00 00 00 00
        //    0x00000468: ult r0.x, r0.y, r0.x            4F 00 00 07 | 12 00 10 00 | 00 00 00 00 | 1A 00 10 00 | 00 00 00 00 | 0A 00 10 00 | 00 00 00 00
        //    0x00000484: discard_z r0.x                  0D 00 00 03 | 0A 00 10 00 | 00 00 00 00

        // private const string discard_z_sig = "0D 00 00 03 ?? 00 ?? 00 00 00 00 00";

        private const string block_sig =
            "38 00 00 07 ?? 00 ?? 00 ?? 00 00 00 ?? ?? ?? 00 ?? 00 00 00 ?? ?? 00 00 00 00 80 41" +
            "40 00 00 05 ?? 00 ?? 00 ?? 00 00 00 ?? 00 ?? 00 ?? 00 00 00" +
            "1C 00 00 05 ?? 00 ?? 00 ?? 00 00 00 ?? 00 ?? 00 ?? 00 00 00" +
            "4F 00 00 07 ?? 00 ?? 00 ?? 00 00 00 ?? 00 ?? 00 ?? 00 00 00 ?? 00 ?? 00 00 00 00 00" +
            "0D 00 00 03 ?? 00 ?? 00 ?? 00 00 00";
        private const int block_float_offset = 6 * 4;
        private const int block_discard_offset = (7 + 5 + 5 + 7) * 4;
        private const string block2_sig =
            "38 00 00 08 ?? 00 ?? 00 ?? 00 00 00 ?? ?? ?? 00 ?? 00 00 00 ?? 00 00 00 ?? ?? 00 00 00 00 80 41" +
            "40 00 00 05 ?? 00 ?? 00 ?? 00 00 00 ?? 00 ?? 00 ?? 00 00 00" +
            "1C 00 00 05 ?? 00 ?? 00 ?? 00 00 00 ?? 00 ?? 00 ?? 00 00 00" +
            "4F 00 00 07 ?? 00 ?? 00 ?? 00 00 00 ?? 00 ?? 00 ?? 00 00 00 ?? 00 ?? 00 00 00 00 00" +
            "0D 00 00 03 ?? 00 ?? 00 ?? 00 00 00";
        private const int block2_float_offset = 7 * 4;
        private const int block2_discard_offset = (8 + 5 + 5 + 7) * 4;

        private static readonly byte[] block_float_replacement = BitConverter.GetBytes(48.0f);

        private static readonly byte[] nop_12x = { 0x3A, 0x00, 0x00, 0x01, 0x3A, 0x00, 0x00, 0x01, 0x3A, 0x00, 0x00, 0x01 };

        public Patcher(Craf archive) { _archive = archive; }

        public Task DumpDiscardPsAsync(IProgress<int> progress)
        {
            return Task.Run(() =>
            {
                if (!Directory.Exists("shaderDump")) Directory.CreateDirectory("shaderDump");

                for (var i = 0; i < _archive.Count(); i++)
                {
                    var vfsPath = _archive.VfsPath(i);
                    string filename = vfsPath.Substring(vfsPath.LastIndexOfAny("/\\".ToCharArray()) + 1);

                    if (vfsPath.EndsWith(".ps.sb"))
                    {
                        string outputPath = "shaderDump/" + filename;

                        var binary = _archive.Get(i);
                        var disassembly = D3DCompiler.Disassemble(binary);

                        if (disassembly.Contains("discard_z"))
                        {
                            File.WriteAllBytes(outputPath, binary);
                            File.WriteAllText(outputPath + ".lst", disassembly);
                        }
                    }
                    progress.Report(i + 1);
                }
            });
        }
        public Task DisableDitheringAsync(IProgress<int> progress)
        {
            return Task.Run(() =>
            {
                for (var i = 0; i < _archive.Count(); i++)
                {
                    var vfsPath = _archive.VfsPath(i);
                    string filename = vfsPath.Substring(vfsPath.LastIndexOfAny("/\\".ToCharArray()) + 1);

                    if (filename.EndsWith(".ps.sb") && filename.StartsWith("g_buffer"))
                    {
                        var binary = _archive.Get(i);
                        var disassembly = D3DCompiler.Disassemble(binary);

                        // TODO improve
                        if (disassembly.Contains("discard_z"))
                        {
                            bool found = false;
                            byte[] newBinary = (byte[])binary.Clone();

                            foreach (var pos in binary.SigScan(block_sig))
                            {
                                Buffer.BlockCopy(nop_12x, 0, newBinary, pos + block_discard_offset, 12);
                                found = true;
                            }

                            foreach (var pos in binary.SigScan(block2_sig))
                            {
                                Buffer.BlockCopy(nop_12x, 0, newBinary, pos + block2_discard_offset, 12);
                                found = true;
                            }

                            if (found)
                            {
                                int[] checksum = DXBCChecksum.DXBCChecksum.CalculateDXBCChecksum(newBinary);
                                Buffer.BlockCopy(checksum, 0, newBinary, 4, 16);
                                _archive.Replace(i, newBinary);
                            }
                        }
                    }
                    progress.Report(i + 1);
                }
            });
        }

        public Task NarrowDitheringAsync(IProgress<int> progress)
        {
            if (!Directory.Exists("missedShaderDump")) Directory.CreateDirectory("missedShaderDump");

            return Task.Run(() =>
            {
                for (var i = 0; i < _archive.Count(); i++)
                {
                    var vfsPath = _archive.VfsPath(i);
                    string filename = vfsPath.Substring(vfsPath.LastIndexOfAny("/\\".ToCharArray()) + 1);

                    if (filename.EndsWith(".ps.sb") && filename.StartsWith("g_buffer"))
                    {
                        var binary = _archive.Get(i);
                        var disassembly = D3DCompiler.Disassemble(binary);

                        // TODO improve
                        if (disassembly.Contains("discard_z"))
                        {
                            bool found = false;
                            byte[] newBinary = (byte[])binary.Clone();

                            foreach (var pos in binary.SigScan(block_sig))
                            {
                                Buffer.BlockCopy(block_float_replacement, 0, newBinary, pos + block_float_offset, 4);
                                found = true;
                            }
                            foreach (var pos in binary.SigScan(block2_sig))
                            {
                                Buffer.BlockCopy(block_float_replacement, 0, newBinary, pos + block2_float_offset, 4);
                                found = true;
                            }

                            if (found)
                            {
                                int[] checksum = DXBCChecksum.DXBCChecksum.CalculateDXBCChecksum(newBinary);
                                Buffer.BlockCopy(checksum, 0, newBinary, 4, 16);
                                _archive.Replace(i, newBinary);
                            }
                            else
                            {
                                string outputPath = "missedShaderDump/" + filename;
                                File.WriteAllBytes(outputPath, binary);
                                File.WriteAllText(outputPath + ".lst", disassembly);
                            }
                        }
                    }
                    progress.Report(i + 1);
                }
            });
        }
    }
}