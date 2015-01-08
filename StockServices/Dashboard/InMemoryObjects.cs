using StockModel;
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
            ExchangeSymbolList = new ExchangeSymbol();
            feeds = new List<Feed>();
            fakeFeeds = new List<Feed[]>();
        }

        public static ExchangeSymbol ExchangeSymbolList { get; set; }
        public static List<Feed> feeds { get; set; }
        public static List<Feed[]> fakeFeeds { get; set; }
    }
}
