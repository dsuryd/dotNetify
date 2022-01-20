using System.Reactive.Subjects;

namespace StockTicker;

public interface IStockTickerService
{
   IObservable<Dictionary<string, double>> StockPrices { get; }

   void AddSymbol(string symbol);
}

public class StockTickerService : IStockTickerService
{
   private readonly Random _random = new();
   private readonly List<string> _symbols = new();
   private readonly Subject<Dictionary<string, double>> _stockPrices = new();

   public IObservable<Dictionary<string, double>> StockPrices => _stockPrices;

   public StockTickerService()
   {
      _ = new Timer(state =>
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