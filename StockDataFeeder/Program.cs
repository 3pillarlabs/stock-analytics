using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FeederInterface.Feeder;
using StockModel;
using StockModel.Master;
using StockServices.Factory;
using FeederInterface.Sender;
using StockServices.DashBoard;
using StockServices.FakeMarketService;
using System.Threading;


namespace StockDataFeeder
{
    class Program
    {
        delegate void MethodDelegate();
        static void Main(string[] args)
        {
            //Loading system startup data for all the exchanges
            List<Exchange> exchanges = new List<Exchange>();
            exchanges.Add(Exchange.FAKE_NASDAQ);

            InMemoryObjects.LoadInMemoryObjects(exchanges);

            //Initiate fake data generation from fake market
            //Later it will also include data generation from google finance
            TimeSpan updateDuration = TimeSpan.FromMilliseconds(Constants.FAKE_DATA_GENERATE_PERIOD);
            FakeDataGenerator.StartFakeDataGeneration(300);


            IFeeder feeder = FeederFactory.GetFeeder(FeederSourceSystem.FAKEMARKET);
            ISender sender = SenderFactory.GetSender(FeederQueueSystem.REDIS_CACHE);

            List<StockModel.Symbol> symbols = InMemoryObjects.ExchangeSymbolList.SingleOrDefault(x => x.Exchange == Exchange.FAKE_NASDAQ).Symbols;
            List<SymbolFeeds> generatedData = new List<SymbolFeeds>();
            List<StockModel.Symbol> symbolList = new List<StockModel.Symbol>();

            int i = 1;
            long deleteTimeFrom = -1;
            long deleteTimeTo = -1;
            long fetchTimeFrom = -1;
            int j;

            while (true)
            {
                Thread.Sleep(300);

                Parallel.ForEach(symbols, (symbol) =>
                {

                    List<Feed> feedList = new List<Feed>();

                    feedList = feeder.GetFeedList(symbol.Id, 1, fetchTimeFrom);      // Get the list of values for a given symbolId of a market for given time-span
                    sender.SendFeed(feedList);

                    if (feedList.Count > 0)
                    {
                        deleteTimeTo = feedList.OrderByDescending(x => x.TimeStamp).Take(1).SingleOrDefault().TimeStamp;
                        deleteTimeFrom = feedList.OrderBy(x => x.TimeStamp).Take(1).SingleOrDefault().TimeStamp;
                        fetchTimeFrom = deleteTimeTo;
                    }

                    j = feeder.DeleteFeedList(symbol.Id, 1, deleteTimeFrom, deleteTimeTo);

                    lock (FakeDataGenerator.thisLock)
                    {
                        generatedData = InMemoryObjects.ExchangeFakeFeeds.Where(x => x.ExchangeId == Convert.ToInt32(Exchange.FAKE_NASDAQ)).SingleOrDefault().ExchangeSymbolFeed;
                        int count = generatedData.Where(x => x.SymbolId == symbol.Id).SingleOrDefault().Feeds.Count();
                        Console.WriteLine(count.ToString());
                    }

                });
                i++;
            }
        }
    }
}
