// Copyright 2018 Niklas K.

// Permission to use, copy, modify, and/or distribute this software for any
// purpose with or without fee is hereby granted, provided that the above
// copyright notice and this permission notice appear in all copies.

// THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES WITH
// REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR ANY SPECIAL, DIRECT,
// INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES WHATSOEVER RESULTING FROM
// LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT, NEGLIGENCE OR
// OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION WITH THE USE OR
// PERFORMANCE OF THIS SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Joveler.ZLibWrapper;

namespace Craf
{
    // I HAVE ONLY TESTED THIS WITH ONE ARCHIVE (datas/shaders/shadergen/autoexternal.earc)
    // Feel free to use as reference but keep in mind that it may not work out of the box.

    // The game's reader appears to be sensitive to alignment. I'm not sure *all* alignment
    // in this code is exactly what the original archiver does, but this does seem
    // to produce equivalent alignment for the archive mentioned above.

    // Signedness is down to what happened to be convenient. I don't know whether
    // the game uses signed or unsigned arithmetic for any of these fields.
    // Except for encryption, where it actually matters.

    // I do not know whether the game requires files of a certain size to be compressed or uncompressed.

    // This may or may not work with archives taken from the console version.
    // I do not have access to those files and have not looked into it.

    // The API is somewhat specific to what I'm actually using this for.
    // THIS NEEDS THE ENTIRE ARCHIVE LOADED INTO MEMORY to access or replace content.
    // So you probably don't want to use this code unmodified.

    //  USAGE
    //    var craf = Craf.Open("path/to/autoexternal.earc");
    //  now available:
    //    craf.Count();
    //    craf.IndexOf("data://foo/bar.bin"); // => 12
    //    craf.VfsPath(12);                   // => "data://foo/bar.bin"
    //    craf.VfsFilename(12);               // => "bar.bin"
    //    await craf.LoadAsync(progress);     // 'progress' gets updated with file ID after each file is read
    //  now available:
    //    craf.Get(12);                       // => byte[] { ... }
    //    craf.CloseReader();
    //  now available:
    //    craf.Replace(12, new byte[] { ...});
    //    craf.Append("who/cares.bin", "data://baz/quux.bin", false, new byte[] { ... });
    //  You can overwrite the original file here, it's no longer locked since CloseReader()
    //    await craf.SaveAsync("path/to/new/autoexternal.earc", progress);
    //  progress gets updated with file ID after each file's body is written

    //              struct HEADER {
    //  /* 00 */        char magic[4]; // 'C','R','A','F'
    //  /* 04 */        ushort minorVersion;
    //  /* 06 */        char majorVersion;
    //  /* 07 */        char encrypted; // 0x80 = yes
    //  /* 08 */        int fileCount;
    //  /* 0C */        uint unk0;
    //  /* 10 */        uint firstEntryOffset;
    //  /* 14 */        uint firstVfsPathOffset;
    //  /* 18 */        uint firstPathOffset;
    //  /* 1C */        uint firstDataOffset; // yeah, they're assuming this is always within the first 32 bit
    //  /* 20 */        char unk1[8];
    //  /* 28 */        int64 archiveKey;
    //  /* 30 */        char unk2[16];
    //              }   // sizeof(HEADER) = 0x40

    //              struct FILEENTRY {
    //  /* 00 */        int64 fileKey;
    //  /* 08 */        int uncompressedSize;
    //  /* 0C */        int totalCompressedSize; // including chunk headers
    //  /* 10 */        uint flags; // 2 = compressed
    //  /* 14 */        uint vfsPathOffset;
    //  /* 18 */        int64 dataOffset;
    //  /* 20 */        uint pathOffset;
    //  /* 24 */        ushort unk0;
    //  /* 26 */        ushort chunkKey;
    //              }   // sizeof(FILEENTRY) = 0x28

    // The encryption code in the game is slightly more complicated than here.
    // The examples I've looked at have only used this parameter subset so that's
    // all that I reversed.
    // 
    // These are the functions responsible for decrypting metadata in the game
    // VAs in Steam original retail release build and search signatures:
    // 140BCF1E0 - 48 8B C4 4C 89 48 20 4C 89 40 18 48 89 48 08 55 56 57 41 54 41 55 41 56 41 57 48 8D 68 B9 48
    // 1488F4C80 - 48 89 5C 24 10 48 89 6C 24 18 48 89 74 24 20 57 41 54 41 55 41 56 41 57 48 83 EC 30 48 8D 41 68


    internal static class Extensions
    {
        public static string ReadNullTerminatedString(this BinaryReader reader)
        {
            var sb = new StringBuilder();
            char lastRead;
            while ((int)(lastRead = reader.ReadChar()) != 0)
            {
                sb.Append(lastRead);
            }
            return sb.ToString();
        }

        public static void WriteNullTerminatedString(this BinaryWriter writer, string str)
        {
            byte[] bstr = Encoding.ASCII.GetBytes(str);
            writer.Write(bstr);
            writer.Write('\0');
        }
    }

    public class CrafArchive
    {
        private class CrafEntry
        {
            public ushort unk0;

            public int uncompressedSize;
            public int totalCompressedSize;
            public uint flags;

            public bool UseCompression { get { return (flags & 2) != 0; } }

            public string path;
            public string vfsPath;
            public CrafChunk[] chunks; // if compressed
            public byte[] data;        // if uncompressed

            public Int64 entryKey;
            public ushort chunkKey;

            // Only used before load and for writing
            public Int64 dataOffset;

            // Only used for writing
            public uint vfsPathOffset;
            public uint pathOffset;
        }

        private class CrafChunk
        {
            public int uncompressedSize;

            public byte[] compressedData;
        }

        public const int ChunkSize = 0x20000;
        public const Int64 MasterArchiveKey = unchecked((Int64)0xCBF29CE484222325);
        public const Int64 MasterEntryKey = unchecked((Int64)0x100000001B3);
        public const Int64 MasterChunkKey1 = unchecked((Int64)0x10E64D70C2A29A69);
        public const Int64 MasterChunkKey2 = unchecked((Int64)0xC63D3dC167E);

        private byte _versionMajor;
        private ushort _versionMinor;
        private byte _useEncryption;

        private uint _unk0;
        private byte[] _unk1;
        private byte[] _unk2;

        private Int64 _archiveKey;

        private List<CrafEntry> _files;

        private FileStream _inputStream;
        private bool _loaded;

        private CrafArchive() { }

        ~CrafArchive()
        {
            if (_inputStream != null) _inputStream.Close();
        }

        public bool MetadataEncrypted { get { return _useEncryption == 0x80; } }

        public static CrafArchive Open(string Path)
        {
            var result = new CrafArchive();
            result._loaded = false;
            result._inputStream = File.OpenRead(Path);
            result.ReadMetadata();

            return result;
        }

        private void ReadMetadata()
        {
            if (_inputStream is null) throw new Exception("Tried to read CRAF metadata after reader was closed");

            using (BinaryReader bin = new BinaryReader(_inputStream, Encoding.Default, true))
            {
                _inputStream.Seek(4, SeekOrigin.Begin);
                _versionMinor = bin.ReadUInt16();
                _versionMajor = bin.ReadByte();
                _useEncryption = bin.ReadByte();
                var fileCount = bin.ReadInt32();
                _files = new List<CrafEntry>(fileCount);
                _unk0 = bin.ReadUInt32();

                uint firstEntryOffset = bin.ReadUInt32();

                _inputStream.Seek(0x20, SeekOrigin.Begin);
                _unk1 = bin.ReadBytes(8);
                _archiveKey = bin.ReadInt64();
                _unk2 = bin.ReadBytes(16);

                Int64 rollingKey = MasterArchiveKey ^ _archiveKey;

                _inputStream.Seek(firstEntryOffset, SeekOrigin.Begin);

                for (var i = 0; i < fileCount; i++)
                {
                    var entry = new CrafEntry();
                    entry.entryKey = bin.ReadInt64();
                    entry.uncompressedSize = bin.ReadInt32();
                    entry.totalCompressedSize = bin.ReadInt32();
                    entry.flags = bin.ReadUInt32();
                    uint vfsPathOffset = bin.ReadUInt32();
                    entry.dataOffset = bin.ReadInt64();
                    uint pathOffset = bin.ReadUInt32();
                    entry.unk0 = bin.ReadUInt16();
                    entry.chunkKey = bin.ReadUInt16();

                    long entryEnd = _inputStream.Position;
                    _inputStream.Seek(vfsPathOffset, SeekOrigin.Begin);
                    entry.vfsPath = bin.ReadNullTerminatedString();
                    _inputStream.Seek(pathOffset, SeekOrigin.Begin);
                    entry.path = bin.ReadNullTerminatedString();
                    _inputStream.Seek(entryEnd, SeekOrigin.Begin);
                    _files.Add(entry);

                    if (MetadataEncrypted)
                    {
                        Int64 fileSizeKey = (rollingKey * MasterEntryKey) ^ entry.entryKey;
                        int uncompressedSizeKey = (int)(fileSizeKey >> 32);
                        int compressedSizeKey = (int)(fileSizeKey & 0xFFFFFFFF);
                        entry.uncompressedSize ^= uncompressedSizeKey;
                        entry.totalCompressedSize ^= compressedSizeKey;
                        Int64 dataOffsetKey = (fileSizeKey * MasterEntryKey) ^ ~(entry.entryKey);
                        entry.dataOffset ^= dataOffsetKey;

                        rollingKey = dataOffsetKey;
                    }
                }
            }
        }

        public void CloseReader()
        {
            if (_inputStream is null) return;
            _inputStream.Close();
            _inputStream = null;
        }

        private int ChunkCount(int uncompressedSize)
        {
            var result = uncompressedSize / ChunkSize;
            if (uncompressedSize % ChunkSize != 0) result++;
            return (int)result;
        }

        private int AlignTo(int offset, int align)
        {
            if (offset % align == 0) return offset;
            return ((offset / align) + 1) * align;
        }
        private uint AlignTo(uint offset, int align)
        {
            if (offset % align == 0) return offset;
            return (uint)(((offset / align) + 1) * align);
        }
        private long AlignTo(long offset, int align)
        {
            if (offset % align == 0) return offset;
            return ((offset / align) + 1) * align;
        }

        private void LoadEntry(int id)
        {
            if (id < 0 || id > _files.Count - 1) throw new Exception("Tried to read CRAF entry out of bounds");
            if (_inputStream is null) throw new Exception("Tried to load CRAF entry after reader was closed");

            using (BinaryReader bin = new BinaryReader(_inputStream, Encoding.Default, true))
            {
                _inputStream.Seek(_files[id].dataOffset, SeekOrigin.Begin);

                if (_files[id].UseCompression)
                {
                    var chunkCount = ChunkCount(_files[id].uncompressedSize);

                    _files[id].chunks = new CrafChunk[chunkCount];

                    for (var j = 0; j < chunkCount; j++)
                    {
                        _files[id].chunks[j] = new CrafChunk();
                        var compressedSize = bin.ReadInt32();
                        _files[id].chunks[j].uncompressedSize = bin.ReadInt32();
                        if (MetadataEncrypted && j == 0)
                        {
                            Int64 chunkKey = (MasterChunkKey1 * _files[id].chunkKey) + MasterChunkKey2;
                            int compressedSizeKey = (int)(chunkKey >> 32);
                            int uncompressedSizeKey = (int)(chunkKey & 0xFFFFFFFF);
                            compressedSize ^= compressedSizeKey;
                            _files[id].chunks[j].uncompressedSize ^= uncompressedSizeKey;
                        }
                        _files[id].chunks[j].compressedData = bin.ReadBytes(compressedSize);
                    }
                }
                else
                {
                    _files[id].data = bin.ReadBytes(_files[id].uncompressedSize);
                }
            }
        }

        public int Count() { return _files.Count; }

        public Task LoadAsync(IProgress<int> progress)
        {
            if (_loaded) return Task.FromResult(0);
            if (_inputStream is null) throw new Exception("Tried to load CRAF data after reader was closed");

            return Task.Run(() =>
            {
                for (var i = 0; i < Count(); i++)
                {
                    LoadEntry(i);
                    progress.Report(i + 1);
                }
                _loaded = true;
            });
        }

        public void Append(string path, string vfsPath, bool compress, byte[] content)
        {
            if (_inputStream != null) throw new Exception("Tried to add CRAF entry while still in read mode");
            if (IndexOf(vfsPath) != -1) throw new Exception("Tried to add file to CRAF with same path as existing file");

            var entry = new CrafEntry();
            entry.path = path;
            entry.vfsPath = vfsPath;
            entry.flags = compress ? (uint)2 : 0;
            _files.Add(entry);
            Replace(_files.Count - 1, content);
        }

        public int IndexOf(string vfsPath)
        {
            return _files.FindIndex((entry) =>
            {
                return entry.vfsPath == vfsPath;
            });
        }

        public int IndexOfDiskPath(string path)
        {
            return _files.FindIndex((entry) =>
            {
                return entry.path == path;
            });
        }

        public void Replace(int id, byte[] content)
        {
            if (id < 0 || id > _files.Count - 1) throw new Exception("Tried to modify CRAF entry out of bounds");
            if (_inputStream != null) throw new Exception("Tried to modify CRAF entry while still in read mode");

            var uncompressedSize = content.Length;
            _files[id].uncompressedSize = uncompressedSize;

            if (_files[id].UseCompression)
            {
                var chunkCount = ChunkCount(content.Length);
                _files[id].chunks = new CrafChunk[chunkCount];
                _files[id].totalCompressedSize = 8 * chunkCount;

                var pos = 0;
                var remaining = uncompressedSize;

                for (var j = 0; j < chunkCount; j++)
                {
                    _files[id].chunks[j] = new CrafChunk();
                    var chunkUncompressedSize = Math.Min(ChunkSize, remaining);
                    _files[id].chunks[j].uncompressedSize = chunkUncompressedSize;
                    
                    using (MemoryStream compressedStream = new MemoryStream())
                    {
                        using (ZLibStream compressor = new ZLibStream(compressedStream,
                                                                      CompressionMode.Compress,
                                                                      CompressionLevel.Best,
                                                                      true))
                        {
                            compressor.Write(content, pos, chunkUncompressedSize);
                        }
                        var chunkCompressedData = compressedStream.ToArray();
                        _files[id].totalCompressedSize += chunkCompressedData.Length;
                        _files[id].chunks[j].compressedData = chunkCompressedData;
                    }

                    pos += chunkUncompressedSize;
                    remaining -= chunkUncompressedSize;
                }

                _files[id].totalCompressedSize = AlignTo(_files[id].totalCompressedSize, 4);
            }
            else
            {
                _files[id].totalCompressedSize = uncompressedSize;
                _files[id].data = content;
            }
        }

        public string VfsPath(int id)
        {
            if (id < 0 || id > _files.Count - 1) throw new Exception("Tried to read CRAF entry out of bounds");

            return _files[id].vfsPath;
        }

        public string VfsFilename(int id)
        {
            if (id < 0 || id > _files.Count - 1) throw new Exception("Tried to read CRAF entry out of bounds");

            var vfsPath = VfsPath(id);
            return vfsPath.Substring(vfsPath.LastIndexOfAny("/\\".ToCharArray()) + 1);
        }

        public string DiskPath(int id)
        {
            if (id < 0 || id > _files.Count - 1) throw new Exception("Tried to read CRAF entry out of bounds");

            return _files[id].path;
        }

        public byte[] Get(int id)
        {
            if (id < 0 || id > _files.Count - 1) throw new Exception("Tried to read CRAF entry out of bounds");
            if (!_loaded) throw new Exception("Tried to read CRAF entry before loading data");

            if (_files[id].UseCompression)
            {
                byte[] result = new byte[_files[id].uncompressedSize];

                var pos = 0;

                for (var j = 0; j < _files[id].chunks.Length; j++)
                {
                    var chunkUncompressedSize = _files[id].chunks[j].uncompressedSize;
                    using (MemoryStream compressedDataStream = new MemoryStream(_files[id].chunks[j].compressedData))
                    {
                        using (ZLibStream decompressor = new ZLibStream(compressedDataStream, CompressionMode.Decompress))
                        {
                            decompressor.Read(result, pos, chunkUncompressedSize);
                        }
                    }
                    pos += chunkUncompressedSize;
                }

                return result;
            }
            else
            {
                return _files[id].data;
            }
        }


        public Task SaveAsync(string Path, IProgress<int> progress)
        {
            if (!_loaded) throw new Exception("Tried to save CRAF before loading data");
            if (_inputStream != null) throw new Exception("Tried to save CRAF while still in read mode");

            return Task.Run(() =>
            {
                // Create mode is slow
                if (File.Exists(Path)) File.Delete(Path);
                using (FileStream file = File.OpenWrite(Path))
                {
                    using (BinaryWriter bin = new BinaryWriter(file))
                    {
                        var magic = new char[] { 'C', 'R', 'A', 'F' };
                        bin.Write(magic);
                        bin.Write(_versionMinor);
                        bin.Write(_versionMajor);
                        bin.Write(_useEncryption);
                        bin.Write(_files.Count);
                        bin.Write(_unk0);
                        file.Seek(0x20, SeekOrigin.Begin);
                        bin.Write(_unk1);
                        bin.Write(_archiveKey);
                        bin.Write(_unk2);

                        uint firstEntryOffset = (uint)file.Position;

                        file.Seek(firstEntryOffset + 0x28 * _files.Count, SeekOrigin.Begin);
                        file.Seek(AlignTo(file.Position, 16), SeekOrigin.Begin);
                        // don't ask me why, but it's normally padded like that
                        file.Seek(8, SeekOrigin.Current);
                        for (var i = 0; i < _files.Count; i++)
                        {
                            var vfsPathOffset = AlignTo(file.Position, 8);
                            file.Seek(vfsPathOffset, SeekOrigin.Begin);
                            bin.WriteNullTerminatedString(_files[i].vfsPath);
                            _files[i].vfsPathOffset = (uint)vfsPathOffset;
                        }

                        // don't ask me why, but it's normally padded like that
                        file.Seek(AlignTo(file.Position, 16), SeekOrigin.Begin);
                        file.Seek(8, SeekOrigin.Current);

                        for (var i = 0; i < _files.Count; i++)
                        {
                            var pathOffset = AlignTo(file.Position, 8);
                            file.Seek(pathOffset, SeekOrigin.Begin);
                            bin.WriteNullTerminatedString(_files[i].path);
                            _files[i].pathOffset = (uint)pathOffset;
                        }

                        for (var i = 0; i < _files.Count; i++)
                        {
                            var dataOffset = AlignTo(file.Position, 0x200);
                            file.Seek(dataOffset, SeekOrigin.Begin);
                            if (_files[i].UseCompression)
                            {
                                for (var j = 0; j < _files[i].chunks.Length; j++)
                                {
                                    var compressedSize = _files[i].chunks[j].compressedData.Length;
                                    var uncompressedSize = _files[i].chunks[j].uncompressedSize;
                                    if (MetadataEncrypted && j == 0)
                                    {
                                        Int64 chunkKey = (MasterChunkKey1 * _files[i].chunkKey) + MasterChunkKey2;
                                        int compressedSizeKey = (int)(chunkKey >> 32);
                                        int uncompressedSizeKey = (int)(chunkKey & 0xFFFFFFFF);
                                        compressedSize ^= compressedSizeKey;
                                        uncompressedSize ^= uncompressedSizeKey;
                                    }
                                    bin.Write(compressedSize);
                                    bin.Write(uncompressedSize);
                                    bin.Write(_files[i].chunks[j].compressedData);
                                }
                                file.Seek(dataOffset + _files[i].totalCompressedSize, SeekOrigin.Begin); // compressed files are padded;
                            }
                            else
                            {
                                bin.Write(_files[i].data);
                            }
                            _files[i].dataOffset = (uint)dataOffset;

                            // files should not directly follow one another
                            file.Seek(1, SeekOrigin.Current);

                            progress.Report(i + 1);
                        }
                        // original files are padded at the end
                        if (AlignTo(file.Position, 0x200) != file.Position)
                        {
                            file.Seek(AlignTo(file.Position, 0x200) - 1, SeekOrigin.Begin);
                            // force filesize increase
                            file.WriteByte(0);
                        }

                        file.Seek(0x10, SeekOrigin.Begin);
                        bin.Write(firstEntryOffset);
                        bin.Write(_files[0].vfsPathOffset);
                        bin.Write(_files[0].pathOffset);
                        bin.Write((int)(_files[0].dataOffset));

                        file.Seek(firstEntryOffset, SeekOrigin.Begin);
                        Int64 rollingKey = MasterArchiveKey ^ _archiveKey;
                        for (var i = 0; i < _files.Count; i++)
                        {
                            var uncompressedSize = _files[i].uncompressedSize;
                            var totalCompressedSize = _files[i].totalCompressedSize;
                            var dataOffset = _files[i].dataOffset;
                            var entryKey = _files[i].entryKey;
                            if (MetadataEncrypted)
                            {
                                Int64 fileSizeKey = (rollingKey * MasterEntryKey) ^ entryKey;
                                int uncompressedSizeKey = (int)(fileSizeKey >> 32);
                                int compressedSizeKey = (int)(fileSizeKey & 0xFFFFFFFF);
                                uncompressedSize ^= uncompressedSizeKey;
                                totalCompressedSize ^= compressedSizeKey;
                                Int64 dataOffsetKey = (fileSizeKey * MasterEntryKey) ^ ~(entryKey);
                                dataOffset ^= dataOffsetKey;

                                rollingKey = dataOffsetKey;
                            }

                            bin.Write(entryKey);
                            bin.Write(uncompressedSize);
                            bin.Write(totalCompressedSize);
                            bin.Write(_files[i].flags);
                            bin.Write(_files[i].vfsPathOffset);
                            bin.Write(dataOffset);
                            bin.Write(_files[i].pathOffset);
                            bin.Write(_files[i].unk0);
                            bin.Write(_files[i].chunkKey);
                        }
                    }
                }
            });
        }
    }
}