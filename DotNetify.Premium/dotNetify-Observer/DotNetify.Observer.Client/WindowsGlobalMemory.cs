using System.Runtime.InteropServices;

namespace DotNetify.Observer
{
   internal static class WindowsGlobalMemory
   {
      [StructLayout(LayoutKind.Sequential)]
      internal struct MemoryStatusEx
      {
         internal uint dwLength;
         private readonly uint dwMemoryLoad;
         internal readonly ulong ullTotalPhys;
         internal readonly ulong ullAvailPhys;
         private readonly ulong ullTotalPageFile;
         private readonly ulong ullAvailPageFile;
         private readonly ulong ullTotalVirtual;
         private readonly ulong ullAvailVirtual;
         private readonly ulong ullAvailExtendedVirtual;
      }

      // Source: https://www.pinvoke.net/default.aspx/kernel32.globalmemorystatusex
      [return: MarshalAs(UnmanagedType.Bool)]
      [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
      private static extern bool GlobalMemoryStatusEx(ref MemoryStatusEx lpBuffer);

      internal static bool GetStatus(out ulong totalMemory, out ulong availableMemory)
      {
         totalMemory = availableMemory = 0;
         var memoryStatus = new MemoryStatusEx { dwLength = (uint) Marshal.SizeOf(typeof(MemoryStatusEx)) };

         var result = GlobalMemoryStatusEx(ref memoryStatus);
         if (result)
         {
            totalMemory = memoryStatus.ullTotalPhys;
            availableMemory = memoryStatus.ullAvailPhys;
         }
         return result;
      }
   }
}