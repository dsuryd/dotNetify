using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DotNetify.Observer
{
   public static class LinuxProcInfo
   {
      internal static bool GetMemoryStatus(out ulong totalMemory, out ulong availableMemory)
      {
         totalMemory = 0;
         availableMemory = 0;

         if (File.Exists("/proc/meminfo"))
         {
            using (var reader = new StreamReader("/proc/meminfo"))
            {
               var line = reader.ReadLine();
               while (line != null && !(totalMemory > 0 && availableMemory > 0))
               {
                  if (line.Contains("MemAvailable:"))
                  {
                     var match = Regex.Match(line, @":\s+(\d+) kB");
                     if (match.Success && match.Groups.Count > 1)
                        ulong.TryParse(match.Groups[1].Value, out availableMemory);
                  }
                  else if (line.Contains("MemTotal:"))
                  {
                     var match = Regex.Match(line, @":\s+(\d+) kB");
                     if (match.Success && match.Groups.Count > 1)
                        ulong.TryParse(match.Groups[1].Value, out totalMemory);
                  }

                  line = reader.ReadLine();
               }
            }
         }

         return totalMemory > 0 && availableMemory > 0;
      }

      internal static bool GetCpuStatus(out ulong totalCpu, out ulong idleCpu)
      {
         totalCpu = 0;
         idleCpu = 0;

         if (File.Exists("/proc/stat"))
         {
            using (var reader = new StreamReader("/proc/stat"))
            {
               var line = reader.ReadLine();
               if (line != null && line.Contains("cpu "))
               {
                  line = line.Substring(3).Trim(); // Remove "cpu".
                  var numbers = line.Split(' ').Select(x => long.Parse(x)).ToArray();
                  totalCpu = (ulong) numbers.Sum();
                  idleCpu = (ulong) numbers[3];
               }
            }
         }

         return totalCpu > 0;
      }
   }
}