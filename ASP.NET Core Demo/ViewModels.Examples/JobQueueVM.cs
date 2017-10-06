using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;
using DotNetify;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ViewModels
{
   /// <summary>
   /// This example demonstrates how the client can execute a job on the server, show a progress bar
   /// and receives the results back asynchronously.
   /// </summary>
   public class JobQueueVM : BaseVM
   {
      private Timer _timer;
      private ConcurrentQueue<JObject> _jobs = new ConcurrentQueue<JObject>();
      private OrderedDictionary _jobProgress = new OrderedDictionary();
      private JObject _currentJob;

      public JObject NewJob
      {
         get { return null; }
         set
         {
            _jobs.Enqueue(value);

            var jobId = value["ID"].Value<string>();
            if (!_jobProgress.Contains(jobId))
               _jobProgress.Add(jobId, 0);
            Changed(() => JobProgress);
         }
      }

      public string JobComplete
      {
         get { return Get<string>(); }
         set { Set(value); }
      }

      public List<object> JobProgress
      {
         get { return _jobProgress.Cast<DictionaryEntry>().Select(i => (object)new { ID = i.Key, Percent = i.Value.ToString() + "%" }).ToList(); }
      }

      /// <summary>
      /// Constructor.
      /// </summary>
      public JobQueueVM()
      {
         _timer = new Timer(Timer_Elapsed, null, 500, 500);
      }

      public override void Dispose()
      {
         _timer.Dispose();

         // Call base.Dispose to raise Disposed event.
         base.Dispose();
      }

      private void Timer_Elapsed(object state)
      {
         if (_currentJob == null)
         {
            if (_jobs.Count > 0)
               _jobs.TryDequeue(out _currentJob);
         }

         if (_currentJob != null)
         {
            var jobId = _currentJob["ID"].Value<string>();
            lock (_jobProgress)
            {
               if (_jobProgress.Contains(jobId))
               {
                  var progress = (int)_jobProgress[jobId];
                  progress += 10;
                  if (progress >= 100)
                  {
                     JobComplete = JsonConvert.SerializeObject(
                        new
                        {
                           ID = jobId,
                           Start = _currentJob["Start"].Value<string>(),
                           End = DateTime.Now.ToString("T")
                        });

                     _jobProgress.Remove(jobId);
                     _currentJob = null;
                  }
                  else
                     _jobProgress[jobId] = progress;
               }
            }

            Changed(() => JobProgress);
            PushUpdates();
         }
      }
   }
}
