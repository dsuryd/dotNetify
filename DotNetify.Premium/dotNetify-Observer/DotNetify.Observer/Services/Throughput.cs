using System;
using System.Threading;

namespace DotNetify.Observer
{
   public class Throughput
   {
      private const double TICKS_PER_SECOND = 10000000;

      private readonly object _lock = new object();
      private long _startTimeTicks = 0;
      private long _endTimeTicks = 0;
      private double _throughput = 0;
      private int _count = 1;

      public void Increment(int value, DateTime timeStamp)
      {
         lock (_lock)
         {
            Interlocked.Add(ref _count, value);
            Interlocked.Exchange(ref _endTimeTicks, timeStamp.Ticks);
            Interlocked.CompareExchange(ref _startTimeTicks, timeStamp.Ticks, 0);
         }
      }

      public double GetThroughput()
      {
         lock (_lock)
         {
            if (_endTimeTicks - _startTimeTicks < TICKS_PER_SECOND)
            {
               if (_endTimeTicks < DateTime.UtcNow.AddSeconds(-2).Ticks)
                  _throughput = 0;

               return _throughput;
            }

            int count = Interlocked.Exchange(ref _count, 1) - 1;
            long startTimeTicks = Interlocked.Exchange(ref _startTimeTicks, _endTimeTicks);

            double delta = (_endTimeTicks - startTimeTicks) / TICKS_PER_SECOND;
            _throughput = Math.Round(count / delta, 2);
            return _throughput;
         }
      }
   }
}