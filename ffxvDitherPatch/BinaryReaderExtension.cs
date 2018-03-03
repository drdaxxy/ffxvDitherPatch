using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ffxvDitherPatch
{
    static class BinaryReaderExtension
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
    }
}
