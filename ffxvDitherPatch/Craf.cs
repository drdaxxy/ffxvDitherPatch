using Ionic.Zlib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ffxvDitherPatch
{
    // THIS IS COMPLETE ENOUGH FOR THE SHADERS BUT I HAVEN'T TESTED IT WITH ANYTHING ELSE
    // Feel free to use as reference but don't expect it to fully support all archives

    //              struct HEADER {
    //  /* 00 */        char magic[4]; // 'C','R','A','F'
    //  /* 04 */        ushort minorVersion;
    //  /* 06 */        ushort majorVersion;
    //  /* 08 */        int fileCount;
    //  /* 0C */        uint unk0;
    //  /* 10 */        uint firstEntryOffset;
    //  /* 14 */        uint firstVfsPathOffset;
    //  /* 18 */        uint firstPathOffset;
    //  /* 1C */        uint firstDataOffset;
    //  /* 20 */        char unk1[32];
    //              }   // sizeof(HEADER) = 0x40

    //              struct FILEENTRY {
    //  /* 00 */        uint unk0;
    //  /* 04 */        uint unk1;
    //  /* 08 */        int uncompressedSize;
    //  /* 0C */        int totalCompressedSize; // including chunk headers
    //  /* 10 */        uint flags; // 2 = compressed
    //  /* 14 */        uint vfsPathOffset;
    //  /* 18 */        uint dataOffset;
    //  /* 1C */        uint unk2;
    //  /* 20 */        uint pathOffset;
    //  /* 24 */        uint unk3;
    //              }   // sizeof(FILEENTRY) = 0x28



    class Craf
    {
        private class CrafEntry
        {
            public uint unk0;
            public uint unk1;
            public uint unk2;
            public uint unk3;

            public int uncompressedSize;
            public int totalCompressedSize;
            public uint flags;

            public bool UseCompression { get { return (flags & 2) != 0; } }

            public string path;
            public string vfsPath;
            public CrafChunk[] chunks; // if compressed
            public byte[] data;        // if uncompressed

            // Only used before load and for writing
            public uint dataOffset;

            // Only used for writing
            public uint vfsPathOffset;
            public uint pathOffset;
        }

        private class CrafChunk
        {
            public int uncompressedSize;

            public byte[] compressedData;
        }

        public const int ChunkSize = 0x200000;

        private ushort _versionMajor;
        private ushort _versionMinor;

        private uint _unk0;
        private byte[] _unk1;

        private CrafEntry[] _files;

        private FileStream _inputStream;
        private bool _loaded;

        private Craf() { }

        ~Craf()
        {
            if (_inputStream != null) _inputStream.Close();
        }

        public static Craf Open(string Path)
        {
            var result = new Craf();
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
                _versionMajor = bin.ReadUInt16();
                _files = new CrafEntry[bin.ReadInt32()];
                _unk0 = bin.ReadUInt32();

                uint firstEntryOffset = bin.ReadUInt32();

                _inputStream.Seek(0x20, SeekOrigin.Begin);
                _unk1 = bin.ReadBytes(32);

                _inputStream.Seek(firstEntryOffset, SeekOrigin.Begin);

                for (var i = 0; i < _files.Length; i++)
                {
                    _files[i] = new CrafEntry();
                    _files[i].unk0 = bin.ReadUInt32();
                    _files[i].unk1 = bin.ReadUInt32();
                    _files[i].uncompressedSize = bin.ReadInt32();
                    _files[i].totalCompressedSize = bin.ReadInt32();
                    _files[i].flags = bin.ReadUInt32();
                    uint vfsPathOffset = bin.ReadUInt32();
                    _files[i].dataOffset = bin.ReadUInt32();
                    _files[i].unk2 = bin.ReadUInt32();
                    uint pathOffset = bin.ReadUInt32();
                    _files[i].unk3 = bin.ReadUInt32();

                    long entryEnd = _inputStream.Position;
                    _inputStream.Seek(vfsPathOffset, SeekOrigin.Begin);
                    _files[i].vfsPath = bin.ReadNullTerminatedString();
                    _inputStream.Seek(pathOffset, SeekOrigin.Begin);
                    _files[i].path = bin.ReadNullTerminatedString();
                    _inputStream.Seek(entryEnd, SeekOrigin.Begin);
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

        private void LoadEntry(int id)
        {
            if (id < 0 || id > _files.Length - 1) throw new Exception("Tried to read CRAF entry out of bounds");
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
                        _files[id].chunks[j].compressedData = bin.ReadBytes(compressedSize);
                    }
                }
                else
                {
                    _files[id].data = bin.ReadBytes(_files[id].uncompressedSize);
                }
            }
        }

        public int Count() { return _files.Length; }

        public Task LoadAsync(IProgress<int> progress)
        {
            if (_loaded) return Task.FromResult(0);
            if (_inputStream is null) throw new Exception("Tried to load CRAF data after reader was closed");

            return Task.Run(() =>
            {
                for (var i = 0; i < Count(); i++)
                {
                    LoadEntry(i);
                    progress.Report(i+1);
                }
                _loaded = true;
            });
        }

        public void Replace(int id, byte[] content)
        {
            if (id < 0 || id > _files.Length - 1) throw new Exception("Tried to modify CRAF entry out of bounds");
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
                        using (ZlibStream compressor = new ZlibStream(
                            compressedStream,
                            CompressionMode.Compress,
                            CompressionLevel.BestCompression,
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

                _files[id].totalCompressedSize += (8 - (_files[id].totalCompressedSize % 8));
            }
            else
            {
                _files[id].totalCompressedSize = uncompressedSize;
                _files[id].data = content;
            }
        }

        public string VfsPath(int id)
        {
            if (id < 0 || id > _files.Length - 1) throw new Exception("Tried to read CRAF entry out of bounds");

            return _files[id].vfsPath;
        }

        public byte[] Get(int id)
        {
            if (id < 0 || id > _files.Length - 1) throw new Exception("Tried to read CRAF entry out of bounds");
            if (!_loaded) throw new Exception("Tried to read CRAF entry before loading data");

            if (_files[id].UseCompression)
            {
                byte[] result = new byte[_files[id].uncompressedSize];

                var pos = 0;

                for (var j = 0; j < _files[id].chunks.Length; j++)
                {
                    var chunkUncompressedSize = _files[id].chunks[j].uncompressedSize;
                    using (MemoryStream decompressedStream = new MemoryStream())
                    {
                        using (ZlibStream decompressor = new ZlibStream(
                            decompressedStream,
                            CompressionMode.Decompress,
                            true))
                        {
                            decompressor.Write(
                                _files[id].chunks[j].compressedData,
                                0,
                                _files[id].chunks[j].compressedData.Length);
                        }
                        decompressedStream.Seek(0, SeekOrigin.Begin);
                        decompressedStream.Read(result, pos, chunkUncompressedSize);
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
                using (FileStream file = File.OpenWrite(Path))
                {
                    using (BinaryWriter bin = new BinaryWriter(file))
                    {
                        var magic = new char[] { 'C', 'R', 'A', 'F' };
                        bin.Write(magic);
                        bin.Write(_versionMinor);
                        bin.Write(_versionMajor);
                        bin.Write(_files.Length);
                        bin.Write(_unk0);
                        file.Seek(0x20, SeekOrigin.Begin);
                        bin.Write(_unk1);

                        uint firstEntryOffset = (uint)file.Position;

                        file.Seek(firstEntryOffset + 0x28 * _files.Length, SeekOrigin.Begin);

                        for (var i = 0; i < _files.Length; i++)
                        {
                            var vfsPathOffset = file.Position + (8 - (file.Position % 8));
                            file.Seek(vfsPathOffset, SeekOrigin.Begin);
                            bin.WriteNullTerminatedString(_files[i].vfsPath);
                            _files[i].vfsPathOffset = (uint)vfsPathOffset;
                        }

                        // don't ask me why, but it's normally padded like that
                        file.Seek((8 - (file.Position % 8)), SeekOrigin.Current);
                        file.Seek(8, SeekOrigin.Current);

                        for (var i = 0; i < _files.Length; i++)
                        {
                            var pathOffset = file.Position + (8 - (file.Position % 8));
                            file.Seek(pathOffset, SeekOrigin.Begin);
                            bin.WriteNullTerminatedString(_files[i].path);
                            _files[i].pathOffset = (uint)pathOffset;
                        }

                        for (var i = 0; i < _files.Length; i++)
                        {
                            var dataOffset = file.Position + (0x400 - (file.Position % 0x400));
                            file.Seek(dataOffset, SeekOrigin.Begin);
                            if (_files[i].UseCompression)
                            {
                                for (var j = 0; j < _files[i].chunks.Length; j++)
                                {
                                    bin.Write(_files[i].chunks[j].compressedData.Length);
                                    bin.Write(_files[i].chunks[j].uncompressedSize);
                                    bin.Write(_files[i].chunks[j].compressedData);

                                    file.Seek(dataOffset + _files[i].totalCompressedSize, SeekOrigin.Begin); // compressed files are padded;
                                }
                            }
                            else
                            {
                                bin.Write(_files[i].data);
                            }
                            _files[i].dataOffset = (uint)dataOffset;

                            progress.Report(i+1);
                        }

                        file.Seek(0x10, SeekOrigin.Begin);
                        bin.Write(firstEntryOffset);
                        bin.Write(_files[0].vfsPathOffset);
                        bin.Write(_files[0].pathOffset);
                        bin.Write(_files[0].dataOffset);

                        file.Seek(firstEntryOffset, SeekOrigin.Begin);
                        for (var i = 0; i < _files.Length; i++)
                        {
                            bin.Write(_files[i].unk0);
                            bin.Write(_files[i].unk1);
                            bin.Write(_files[i].uncompressedSize);
                            bin.Write(_files[i].totalCompressedSize);
                            bin.Write(_files[i].flags);
                            bin.Write(_files[i].vfsPathOffset);
                            bin.Write(_files[i].dataOffset);
                            bin.Write(_files[i].unk2);
                            bin.Write(_files[i].pathOffset);
                            bin.Write(_files[i].unk3);
                        }
                    }
                }
            });
        }
    }
}
