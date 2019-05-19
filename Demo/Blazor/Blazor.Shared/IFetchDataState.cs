namespace Blazor.Shared
{
    public interface IFetchDataState
    {
        WeatherForecast[] Forecasts { get; }
    }
}