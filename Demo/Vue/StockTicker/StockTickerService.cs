using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace StockTicker
{
   public interface IStockTickerService
   {
      IObservable<Dictionary<string, double>> StockPrices { get; }

      void AddSymbol(string symbol);
   }

   public class StockTickerService : IStockTickerService
   {
      private readonly Random _random = new Random();
      private readonly List<string> _symbols = new List<string>();
      private readonly Subject<Dictionary<string, double>> _stockPrices = new Subject<Dictionary<string, double>>();

      public IObservable<Dictionary<string, double>> StockPrices => _stockPrices;

      public StockTickerService()
      {
         new Timer(state =>
         {
            var prices = _symbols
               .Select(x => KeyValuePair.Create(x, Math.Round(1000 * _random.NextDouble(), 2)))
               .ToDictionary(x => x.Key, y => y.Value);

            _stockPrices.OnNext(prices);
         }, null, 0, 1000);
      }

      public void AddSymbol(string symbol) 
      {
        if (!_symbols.Contains(symbol)) 
           _symbols.Add(symbol);
      }
   }
}