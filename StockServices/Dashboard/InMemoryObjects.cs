using StockModel;
using StockModel.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServices.DashBoard
{
    public static class InMemoryObjects
    {
        static InMemoryObjects()
        {
            ExchangeSymbolList = new List<ExchangeSymbol>();
         
        }

        public static List<ExchangeSymbol> ExchangeSymbolList { get; set; }
        
        public static List<ExchangeFeeds> ExchangeFakeFeeds { get; set; }


        public static void LoadInMemoryObjects(List<Exchange> exchanges)
        {
            //Load all symbols for all configured market
            LoadInMemoryExchangeSymbols(exchanges);
            LoadInMemoryExchangeFeeds(exchanges);
        }

        public static void LoadInMemoryExchangeSymbols(List<Exchange> exchanges)
        {
            
            ExchangeSymbolList = SymbolService.GetSymbols(exchanges);
        }

        public static void LoadInMemoryExchangeFeeds(List<Exchange> exchanges)
        {
          
            ExchangeFakeFeeds = new List<ExchangeFeeds>();

            foreach (Exchange exchange in exchanges)
            {
                ExchangeFeeds exchangeFeed = new ExchangeFeeds();
                exchangeFeed.ExchangeId = Convert.ToInt32(exchange);
                exchangeFeed.ExchangeSymbolFeed = new List<SymbolFeeds>();

                if (ExchangeSymbolList.Count == 0) LoadInMemoryExchangeSymbols(exchanges);

                foreach (Symbol symbol in ExchangeSymbolList.SingleOrDefault(x => x.Exchange == exchange).Symbols)
                {
                    SymbolFeeds symbolFeeds = new SymbolFeeds();
                    symbolFeeds.SymbolId = symbol.Id;
                    symbolFeeds.Feeds = new List<Feed>();
                    exchangeFeed.ExchangeSymbolFeed.Add(symbolFeeds);
                }
                ExchangeFakeFeeds.Add(exchangeFeed);

            }
        }


    }
}
