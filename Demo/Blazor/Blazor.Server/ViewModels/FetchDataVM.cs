using Blazor.Shared;
using DotNetify;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.Server
{
   public class FetchDataVM : BaseVM
   {
      private static string[] Summaries = new[]
      {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

      public IEnumerable<WeatherForecast> Forecasts => WeatherForecasts();

      private IEnumerable<WeatherForecast> WeatherForecasts()
      {
         var rng = new Random();
         return Enumerable.Range(1, 5).Select(index => new WeatherForecast
         {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = rng.Next(-20, 55),
            Summary = Summaries[rng.Next(Summaries.Length)]
         });
      }
   }
}