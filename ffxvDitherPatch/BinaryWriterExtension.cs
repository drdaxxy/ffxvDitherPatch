using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ffxvDitherPatch
{
    static class BinaryWriterExtension
    {
        public static void WriteNullTerminatedString(this BinaryWriter writer, string str)
        {
            byte[] bstr = Encoding.ASCII.GetBytes(str);
            writer.Write(bstr);
            writer.Write('\0');
        }
    }
}
