using System;
using System.Collections.Generic;
using DotNetify;

namespace StockTicker
{
   public class StockTicker : BaseVM
   {
      private readonly IStockTickerService _service;

      public Dictionary<string, double> StockPrices { get; set; }

      public StockTicker(IStockTickerService service)
      {
         _service = service;
         _service.StockPrices
            .Subscribe(prices =>
            {
               StockPrices = prices;
               Changed(nameof(StockPrices));
               PushUpdates();
            });
      }

      public void AddSymbol(string symbol) => _service.AddSymbol(symbol);
   }
}