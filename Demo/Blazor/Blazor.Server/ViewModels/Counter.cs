using Blazor.Shared;
using DotNetify;
using System.Windows.Input;

namespace Blazor.Server
{
    public class Counter : BaseVM, ICounterState
    {
        public int CurrentCount
        {
            get => Get<int>();
            set => Set(value);
        }

        public ICommand IncrementCount => new Command(() => CurrentCount++);
    }
}