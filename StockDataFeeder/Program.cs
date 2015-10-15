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
using System.Configuration;
using StockServices.PollingYahooMarketService;

namespace StockDataFeeder
{
    class Program
    {
        delegate void MethodDelegate();
        static void Main(string[] args)
        {
            //Loading system startup data for all the exchanges
            List<Exchange> exchanges = new List<Exchange>();

            //default selected...
            Exchange selectedExchange = Exchange.FAKE_NASDAQ;

            ResolveAppArgs(args, selectedExchange);

            exchanges = Enum.GetValues(typeof(Exchange)).OfType<Exchange>().ToList();

            InMemoryObjects.LoadInMemoryObjects(exchanges);

            //Initiate fake data generation from fake market
            //Later it will also include data generation from google finance
            TimeSpan updateDuration = TimeSpan.FromMilliseconds(Constants.FAKE_DATA_GENERATE_PERIOD);
            
            FeederSourceSystem configuredFeeder;
            IFeeder feeder = null; 

            if (Enum.TryParse(ConfigurationManager.AppSettings["Feeder"].ToString(), out configuredFeeder))
            {
                feeder = FeederFactory.GetFeeder(configuredFeeder);
                switch(configuredFeeder)
                {
                    case FeederSourceSystem.YAHOO:
                        YahooDataGenerator.StartDataGeneration(300, selectedExchange);
                        break;
                    case FeederSourceSystem.FAKEMARKET:
                    default:
                        FakeDataGenerator.StartFakeDataGeneration(300);
                        break;
                }
            }

            ISender sender = SenderFactory.GetSender(FeederQueueSystem.REDIS_CACHE);

            List<StockModel.Symbol> symbols = InMemoryObjects.ExchangeSymbolList.SingleOrDefault(x => x.Exchange == selectedExchange).Symbols;
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

                    feedList = feeder.GetFeedList(symbol.Id, (int)selectedExchange, fetchTimeFrom);      // Get the list of values for a given symbolId of a market for given time-span
                    sender.SendFeed(feedList);

                    if (feedList.Count > 0)
                    {
                        deleteTimeTo = feedList.OrderByDescending(x => x.TimeStamp).Take(1).SingleOrDefault().TimeStamp;
                        deleteTimeFrom = feedList.OrderBy(x => x.TimeStamp).Take(1).SingleOrDefault().TimeStamp;
                        fetchTimeFrom = deleteTimeTo;
                    }

                    j = feeder.DeleteFeedList(symbol.Id, (int)selectedExchange, deleteTimeFrom, deleteTimeTo);

                    lock (FakeDataGenerator.thisLock)
                    {
                        generatedData = InMemoryObjects.ExchangeFakeFeeds.Where(x => x.ExchangeId == Convert.ToInt32(selectedExchange)).SingleOrDefault().ExchangeSymbolFeed;
                        int count = generatedData.Where(x => x.SymbolId == symbol.Id).SingleOrDefault().Feeds.Count();
                        Console.WriteLine(count.ToString());
                    }

                });
                i++;
            }
        }

        private static void ResolveAppArgs(string[] args, Exchange selectedExchange)
        {
            if (args != null && args.Length > 0)
            {
                if(!Enum.TryParse(args[0], out selectedExchange))
                {
                    selectedExchange = Exchange.FAKE_NASDAQ;
                }
            }
        }
    }
}
