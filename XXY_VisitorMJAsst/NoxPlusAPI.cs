using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace XXY_VisitorMJAsst
{
    class NoxPlusAPI
    {
        [DllImport("NoxPApp.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NoxFind(int appID, int[] keyHandles, int[] keyNumber);

        [DllImport("NoxPApp.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NoxOpen(int keyHandle, string userPin);

        [DllImport("NoxPApp.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NoxReadStorage(int keyHandle, StringBuilder pBuffer);

        [DllImport("NoxPApp.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NoxWriteStorage(int keyHandle, string pBuffer);

        [DllImport("NoxPApp.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NoxReadMem(int keyHandle, StringBuilder pBuffer);

        [DllImport("NoxPApp.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NoxWriteMem(int keyHandle, string pBuffer);

        [DllImport("NoxPApp.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int NoxClose(int keyHandle);
    }
}
