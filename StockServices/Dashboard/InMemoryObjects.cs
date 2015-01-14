using StockModel;
using StockModel.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServices.Dashboard
{
    public static class InMemoryObjects
    {
        static InMemoryObjects()
        {
            ExchangeSymbolList = new List<ExchangeSymbol>();
            Feeds = new List<Feed>();
            FakeFeeds = new List<Feed[]>();
        }

        public static List<ExchangeSymbol> ExchangeSymbolList { get; set; }
        public static List<Feed> Feeds { get; set; }
        public static List<Feed[]> FakeFeeds { get; set; }

        public static List<ExchangeFeeds> ExchangeFakeFeeds { get; set; }


        public static void LoadInMemoryObjects()
        {
            //Load all symbols for all configured market
            List<Exchange> exchanges = new List<Exchange>();
            exchanges.Add(Exchange.FAKE_NASDAQ);
            ExchangeSymbolList =SymbolService.GetSymbols(exchanges);

            ExchangeFakeFeeds = new List<ExchangeFeeds>();

            foreach (Exchange exchange in exchanges)
            {
                ExchangeFeeds exchangeFeed = new ExchangeFeeds();
                exchangeFeed.ExchangeId = Convert.ToInt32(exchange);
                exchangeFeed.ExchaneSymbolFeed = new List<SymbolFeeds>();
                
                foreach (Symbol symbol in ExchangeSymbolList.SingleOrDefault(x => x.Exchange == exchange).Symbols)
                {
                    SymbolFeeds symbolFeeds = new SymbolFeeds();
                    symbolFeeds.SymbolId = symbol.Id;
                    symbolFeeds.Feeds = new List<Feed>();
                    exchangeFeed.ExchaneSymbolFeed.Add(symbolFeeds);
                }
                ExchangeFakeFeeds.Add(exchangeFeed);
                 
            }

        }

    }
}
