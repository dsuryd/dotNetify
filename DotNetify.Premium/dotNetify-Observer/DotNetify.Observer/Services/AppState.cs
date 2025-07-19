using System;
using System.Reactive.Subjects;

namespace DotNetify.Observer
{
   public interface IAppState
   {
      ISubject<string> SelectedNodeId { get; }
   }

   public class AppState : IAppState
   {
      public ISubject<string> SelectedNodeId { get; } = new BehaviorSubject<string>(null);
   }
}