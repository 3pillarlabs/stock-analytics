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
using StockInterface.Feeder;

namespace StockDataFeeder
{
    class Program
    {
        /// <summary>
        /// Application arguments:
        /// arg0: Selected Exchange. Has to be enum StockModel.Master.Exchange
        /// arg1: Data generator to use. Has to be IDataPublisher.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Loading system startup data for all the exchanges
            List<Exchange> exchanges = new List<Exchange>();

            //defaults selected...
            Exchange selectedExchange = Exchange.FAKE_NASDAQ;
            IDataPublisher dataGenerator = YahooDataGenerator.Instance;

            ResolveAppArgs(args, selectedExchange);

            exchanges = Enum.GetValues(typeof(Exchange)).OfType<Exchange>().ToList();

            InMemoryObjects.LoadInMemoryObjects(exchanges);
            
            TimeSpan updateDuration = TimeSpan.FromMilliseconds(Constants.FAKE_DATA_GENERATE_PERIOD);

            //Start data generation - this will start fetching data for all symbols of current exchange
            //Later, need to change this to only subscribe to the specific symbol(s) selected.
            dataGenerator.StartDataGeneration(300, selectedExchange);

            ISender sender = SenderFactory.GetSender(FeederQueueSystem.REDIS_CACHE);

            List<StockModel.Symbol> symbols = InMemoryObjects.ExchangeSymbolList.SingleOrDefault(x => x.Exchange == selectedExchange).Symbols;

            List<SymbolFeeds> generatedData = new List<SymbolFeeds>();
            List<StockModel.Symbol> symbolList = new List<StockModel.Symbol>();


            Parallel.ForEach(symbols, (symbol) =>
            {
                //subscribe
                dataGenerator.SubscribeFeed(symbol.Id, (Feed fd) => {
                    sender.SendFeed(new List<Feed>() {fd});

                    lock (FakeDataGenerator.thisLock)
                    {
                        generatedData = InMemoryObjects.ExchangeFakeFeeds.Where(x => x.ExchangeId == Convert.ToInt32(selectedExchange)).SingleOrDefault().ExchangeSymbolFeed;
                        int count = generatedData.Where(x => x.SymbolId == symbol.Id).SingleOrDefault().Feeds.Count();
                        Console.WriteLine(count.ToString());
                    }
                });
                List<Feed> feedList = new List<Feed>();
                

            });

            Console.Read();
        }

        private static void ResolveAppArgs(string[] args, Exchange selectedExchange)
        {
            if (args != null)
            {
                if (args.Length > 0)
                {
                    if (!Enum.TryParse(args[0], out selectedExchange))
                    {
                        selectedExchange = Exchange.FAKE_NASDAQ;
                    }
                }
            }
        }
    }
}
