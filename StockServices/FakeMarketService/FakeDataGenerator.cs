using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockModel;
using StockModel.Master;
using StockServices.Dashboard;
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
        public static List<Feed[]> feedsList = new List<Feed[]>();
        public static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);


        public FakeDataGenerator()
        {

        }

        public static void StartFakeDataGeneration(TimeSpan updateDuration)
        {


            Timer dataGenerateTimer = new Timer(GenerateData, null, TimeSpan.FromMilliseconds(0), updateDuration);
        }

        public static void GenerateData(object state)
        {
            // Method to generate feeds and update the in memory objects
            List<StockModel.Symbol> symbols = InMemoryObjects.ExchangeSymbolList.SingleOrDefault(x => x.Exchange == Exchange.FAKE_NASDAQ).Symbols;
            TimeSpan _updateInterval = TimeSpan.FromMilliseconds(Constants.FAKE_DATA_GENERATE_INTERVAL);
            UpdateData(symbols);
            Timer timer = new Timer(UpdateData, symbols, _updateInterval, _updateInterval); // Timer to update the stock-values after every given time-interval
        }


        private static void UpdateData(object state)
        {
            // Method to change the values of all the stocks randomly in a fixed range 
            List<StockModel.Symbol> symbols = (List<StockModel.Symbol>)state;
            Feed[] feedsArray = new Feed[symbols.Count];

            List<SymbolFeeds> symbolFeeds = new List<SymbolFeeds>();

            Parallel.ForEach(symbols, (symbol) =>
            {
                SymbolFeeds feeds = new SymbolFeeds();
                feeds.SymbolId = symbol.Id;


                double changePercent = random.NextDouble() * (Constants.MAX_CHANGE_PERC - Constants.MIN_CHANGE_PERC) + Constants.MIN_CHANGE_PERC;
                symbol.DefaultVal = symbol.DefaultVal + symbol.DefaultVal * changePercent / 100;
                Feed feed = new Feed();
                feed.SymbolId = symbol.Id;
                feed.LTP = symbol.DefaultVal;

                //Unique id for each feed will be generated later 
                //feed.Id = i;

                feed.TimeStamp = Convert.ToInt64((DateTime.Now - epoch).TotalMilliseconds);

                InMemoryObjects.ExchangeFakeFeeds.Where(x => x.ExchangeId == Convert.ToInt32(Exchange.FAKE_NASDAQ)).Take(1).SingleOrDefault().ExchaneSymbolFeed.Where(x => x.SymbolId == symbol.Id).SingleOrDefault().Feeds.Add(feed);

            });


        }
    }
}
