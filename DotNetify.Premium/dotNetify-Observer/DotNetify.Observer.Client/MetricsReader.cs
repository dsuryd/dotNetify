using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DotNetify.Observer
{
   internal class MetricsReader
   {
      private double _lastTotalProcessorTime;
      private DateTime _lastTotalProcessorTimeStamp;
      private PerformanceCounter _perfCounter;
      private ulong _lastTotalCpu;
      private ulong _lastIdleCpu;

      public double? CpuUsage => CalculateCpuUsage();

      public double? MemoryUsage => CalculateMemoryUsage();

      public double? TotalCpuUsage => CalculateTotalCpuUsage();

      public double? TotalMemoryUsage => CalculateTotalMemoryUsage();

      public MetricsReader()
      {
         _lastTotalProcessorTimeStamp = DateTime.UtcNow;
         _lastTotalProcessorTime = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;

         if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
         {
            _perfCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _perfCounter.NextValue();
         }
         else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
         {
            LinuxProcInfo.GetCpuStatus(out _lastTotalCpu, out _lastIdleCpu);
         }
      }

      private double? CalculateCpuUsage()
      {
         double currentTotalProcessorTime = Process.GetCurrentProcess().TotalProcessorTime.TotalMilliseconds;
         double totalCpuTimeUsed = currentTotalProcessorTime - _lastTotalProcessorTime;
         double cpuTimeElapsed = (DateTime.UtcNow - _lastTotalProcessorTimeStamp).TotalMilliseconds * Environment.ProcessorCount;

         _lastTotalProcessorTime = currentTotalProcessorTime;
         _lastTotalProcessorTimeStamp = DateTime.UtcNow;

         return (totalCpuTimeUsed / cpuTimeElapsed).ToPercent();
      }

      private double? CalculateTotalCpuUsage()
      {
         if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Math.Round(_perfCounter.NextValue(), 2);
         else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
         {
            if (LinuxProcInfo.GetCpuStatus(out ulong totalCpu, out ulong idleCpu))
            {
               var deltaTotalCpu = totalCpu - _lastTotalCpu;
               var deltaIdleCpu = idleCpu - _lastIdleCpu;

               _lastTotalCpu = totalCpu;
               _lastIdleCpu = idleCpu;
               return (1.0 - (double) deltaIdleCpu / deltaTotalCpu).ToPercent();
            }
         }

         return null;
      }

      private double? CalculateMemoryUsage()
      {
         return Process.GetCurrentProcess().WorkingSet64.ToMBytes();
      }

      private double? CalculateTotalMemoryUsage()
      {
         if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
         {
            if (WindowsGlobalMemory.GetStatus(out ulong total, out ulong available))
               return ((double) (total - available) / total).ToPercent();
         }
         else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
         {
            if (LinuxProcInfo.GetMemoryStatus(out ulong total, out ulong available))
               return ((double) (total - available) / total).ToPercent();
         }
         return null;
      }
   }

   internal static class ProcessDataExtensions
   {
      private static readonly double MB = Math.Pow(1024, 2);

      public static double ToMBytes(this long number) => Math.Round(number / MB, 2);

      public static double ToPercent(this double number) => Math.Round(number * 100, 2);
   }
}