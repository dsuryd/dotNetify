using System;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetify.Testing
{
   /// <summary>
   /// Emulates a communication action/reaction cycle between dotNetify client and server.
   /// </summary>
   public class ClientSession
   {
      internal const int WAIT_TIMEOUT = 1000;
      internal const int MAX_RESPONSES = 1;

      private readonly Func<Task> _request;
      private readonly IObservable<object[]> _response;
      private readonly AutoResetEvent _waitEvent = new AutoResetEvent(false);
      private EmulationResponses _responses;
      private IDisposable _subscription;
      private int _maxResponses = MAX_RESPONSES;

      /// <summary>
      /// Constructor.
      /// </summary>
      /// <param name="request">Action to run that would evoke server responses.</param>
      /// <param name="response">Object to receive server responses.</param>
      public ClientSession(Func<Task> request, IObservable<object[]> response)
      {
         _request = request;
         _response = response;
      }

      /// <summary>
      /// Runs the communication.
      /// </summary>
      /// <param name="maxResponses">Wait until reaching this number of responses.</param>
      /// <param name="waitTimeout">Wait until timeout in mulliseconds.</param>
      public EmulationResponses Run(int maxResponses = MAX_RESPONSES, int waitTimeout = WAIT_TIMEOUT)
      {
         _responses = new EmulationResponses();
         _subscription = _response.Subscribe(args => RecordResponse(args));
         _maxResponses = maxResponses;

         try
         {
            _request.Invoke().GetAwaiter().GetResult();
         }
         catch (Exception ex)
         {
            _responses.Exception = ex;
            return _responses;
         }

         _waitEvent.WaitOne(waitTimeout);
         _subscription.Dispose();
         return _responses;
      }

      /// <summary>
      /// Runs the communication asynchronously.
      /// </summary>
      /// <param name="maxResponses">Wait until reaching this number of responses.</param>
      /// <param name="waitTimeout">Wait until timeout in mulliseconds.</param>
      public async Task<EmulationResponses> RunAsync(int maxResponses = MAX_RESPONSES, int waitTimeout = WAIT_TIMEOUT)
      {
         _responses = new EmulationResponses();
         _subscription = _response.Subscribe(args => RecordResponse(args));
         _maxResponses = maxResponses;

         try
         {
            await _request.Invoke();
         }
         catch (Exception ex)
         {
            _responses.Exception = ex;
            return _responses;
         }

         await Task.Run(() =>
         {
            _waitEvent.WaitOne(waitTimeout);
            _subscription.Dispose();
         });

         return _responses;
      }

      /// <summary>
      /// Records responses.
      /// </summary>
      /// <param name="args">Response data from the SignalR hub emulator.</param>
      private void RecordResponse(object[] args)
      {
         _responses.Add(new EmulationResponse(args));
         if (_responses.Count >= _maxResponses)
            _waitEvent.Set();
      }
   }
}