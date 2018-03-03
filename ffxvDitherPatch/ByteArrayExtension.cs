using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ffxvDitherPatch
{
    static class ByteArrayExtension
    {
        public static int[] SigScan(this byte[] haystack, string pattern)
        {
            List<int> matches = new List<int>();

            pattern = new string(pattern.ToCharArray().Where((c) => !Char.IsWhiteSpace(c)).ToArray());
            if (pattern.Length % 2 != 0) throw new Exception("malformatted search pattern");

            byte[] needle = new byte[pattern.Length / 2];
            bool[] mask = new bool[pattern.Length / 2];

            for (var i = 0; i < pattern.Length; i += 2)
            {
                string part = pattern.Substring(i, 2);
                if (part == "??")
                {
                    needle[i / 2] = 0;
                    mask[i / 2] = false;
                }
                else
                {
                    needle[i / 2] = byte.Parse(part, System.Globalization.NumberStyles.HexNumber);
                    mask[i / 2] = true;
                }
            }
            for (var i = 0; i <= haystack.Length - needle.Length; i++)
            {
                bool good = true;
                for (var j = 0; j < needle.Length; j++)
                {
                    if (mask[j] && haystack[i + j] != needle[j])
                    {
                        good = false;
                        break;
                    }
                }
                if (good) matches.Add(i);
            }

            return matches.ToArray();
        }
    }
}
