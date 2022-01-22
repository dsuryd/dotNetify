using System.Reactive.Subjects;
using System.Reactive.Linq;
using StockPriceDict = System.Collections.Generic.Dictionary<string, double>;

namespace StockTicker;

public interface IStockTickerService
{
   IObservable<StockPriceDict> StockPrices { get; }

   void AddSymbol(string symbol);
}

public class StockTickerService : IStockTickerService
{
   private readonly Subject<StockPriceDict> _stockPrices = new();
   private readonly List<string> _symbols = new();
   private readonly Random _random = new();

   public IObservable<StockPriceDict> StockPrices => _stockPrices;

   public StockTickerService()
   {
      Observable.Interval(TimeSpan.FromSeconds(1))
         .Select(_ => _symbols
            .Select(x => KeyValuePair.Create(x, Math.Round(1000 * _random.NextDouble(), 2)))
            .ToDictionary(x => x.Key, y => y.Value))
         .Subscribe(_stockPrices);
   }

   public void AddSymbol(string symbol)
   {
      if (!_symbols.Contains(symbol))
         _symbols.Add(symbol);
   }
}