using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockModel;
using StockModel.Master;
using StockServices.DashBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockServices.FakeMarketService
{
    public class FakeDataGenerator
    {
        public static Random random = new Random();
        private static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static Object thisLock = new Object();
        private static int updateDurationTime = 300; 
        private delegate void UpdateDataDelegate();

        public FakeDataGenerator()
        {

        }

        public static void StartFakeDataGeneration(int updateTimePeriod)
        {
            updateDurationTime = updateTimePeriod;
            UpdateDataDelegate del = new UpdateDataDelegate(GenerateData);
            del.BeginInvoke(null, null);
        }

        public static void GenerateData()
        {
            // Method to generate feeds and update the in memory objects
            List<StockModel.Symbol> symbols = InMemoryObjects.ExchangeSymbolList.SingleOrDefault(x => x.Exchange == Exchange.FAKE_NASDAQ).Symbols;
            UpdateData(symbols);
        }


        private static void UpdateData(object state)
        {
            // Method to change the values of all the stocks randomly in a fixed range 
            List<StockModel.Symbol> symbols = (List<StockModel.Symbol>)state;
            Feed[] feedsArray = new Feed[symbols.Count];

            List<SymbolFeeds> symbolFeeds = new List<SymbolFeeds>();

            while (true)
            {
                Thread.Sleep(updateDurationTime);

                Parallel.ForEach(symbols, (symbol) =>
                {
                    SymbolFeeds feeds = new SymbolFeeds();
                    feeds.SymbolId = symbol.Id;


                    double changePercent = random.NextDouble() * (Constants.MAX_CHANGE_PERC - Constants.MIN_CHANGE_PERC) + Constants.MIN_CHANGE_PERC;
                    symbol.DefaultVal = symbol.DefaultVal + symbol.DefaultVal * changePercent / 100;
                    Feed feed = new Feed();
                    feed.SymbolId = symbol.Id;
                    feed.LTP = symbol.DefaultVal;

             
                    feed.TimeStamp = Convert.ToInt64((DateTime.Now - epoch).TotalMilliseconds);

                    //locking the static collection as it will be read from several sources, causing synchroization issues
                    lock (thisLock)
                    {
                        InMemoryObjects.ExchangeFakeFeeds.Where(x => x.ExchangeId == Convert.ToInt32(Exchange.FAKE_NASDAQ)).Take(1).SingleOrDefault().ExchangeSymbolFeed.Where(x => x.SymbolId == symbol.Id).SingleOrDefault().Feeds.Add(feed);
                    }
                });

            }

        }
    }
}
