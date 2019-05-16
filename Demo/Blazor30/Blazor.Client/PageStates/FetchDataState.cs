using Blazor.Shared;

namespace Blazor.Client.PageStates
{
    public class FetchDataState : IFetchDataState
    {
        public WeatherForecast[] Forecasts { get; set; }
    }
}