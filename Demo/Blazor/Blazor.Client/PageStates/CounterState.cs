using Blazor.Shared;

namespace Blazor.Client.PageStates
{
    public class CounterState : ICounterState
    {
        public int CurrentCount { get; set; }
    }
}