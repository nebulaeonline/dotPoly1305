using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPoly1305
{
    internal class Poly1305Interop
    {

        [DllImport("poly1305", CallingConvention = CallingConvention.Cdecl)]
        public static extern int poly1305_auth(
            byte[] tag,
            byte[] message,
            UIntPtr messageLength,
            byte[] key);

        [DllImport("poly1305", EntryPoint = "poly1305_auth", CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int poly1305_auth_ptr(
            byte* tag,
            byte* message,
            UIntPtr messageLength,
            byte* key);
    }
}
