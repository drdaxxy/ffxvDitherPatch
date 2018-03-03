using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ffxvDitherPatch
{
    static class D3DCompiler
    {
        // TODO fix memory leaks in here maybe

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("8BA5FB08-5195-40E2-AC58-0D989C3A0102")]
        private interface ID3DBlob
        {
            [PreserveSig]
            IntPtr GetBufferPointer();
            [PreserveSig]
            UIntPtr GetBufferSize();
        }

        private static string ID3DBlobToString(ID3DBlob blob)
        {
            if (blob is null) return "";

            // yeah great 64-bit support there thanks MS
            return Marshal.PtrToStringAnsi(blob.GetBufferPointer(), (int)blob.GetBufferSize());
        }

        [DllImport("d3dcompiler_47.dll", CallingConvention = CallingConvention.Winapi, SetLastError = false, CharSet = CharSet.Ansi, ExactSpelling = true)]
        private extern static int D3DDisassemble(IntPtr pSrcData,
            UIntPtr SrcDataSize,
            uint Flags,
            [MarshalAs(UnmanagedType.LPStr)] string szComments,
            [MarshalAs(UnmanagedType.Interface), Out] out ID3DBlob Disassembly);

        // TODO error codes
        public static string Disassemble(byte[] binary)
        {
            IntPtr _binary = Marshal.AllocHGlobal(binary.Length);
            Marshal.Copy(binary, 0, _binary, binary.Length);
            var retcode = D3DDisassemble(_binary, (UIntPtr)binary.Length, 0, null, out ID3DBlob blob);
            if (retcode != 0) throw new Exception(string.Format("D3DDisassemble returned error 0x{0}", retcode.ToString("X8")));
            var result = ID3DBlobToString(blob);
            Marshal.FreeHGlobal(_binary);
            return result;
        }
    }
}
