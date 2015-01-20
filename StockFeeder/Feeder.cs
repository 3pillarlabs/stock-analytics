using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using StockFeeder;
using FeederInterface.Sender;
using StockServices.Factory;
using StockModel.Master;
using FeederInterface.Feeder;
using StockModel;
using StockServices.DashBoard;
using StockServices.FakeMarketService;
using System.Timers;

namespace StockFeeder
{
    public partial class Feeder : ServiceBase
    {
        delegate void MethodDelegate();
        List<Feed> feedList = new List<Feed>();
        List<List<List<Feed>>> list_feedList = new List<List<List<Feed>>>();

        public Feeder()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            MethodDelegate metho = new MethodDelegate(this.start);
            metho.BeginInvoke(null, null);
        }
        protected override void OnStop()
        {
        }

        private void start()
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

            while (true)
            {
                Parallel.ForEach(symbols, (symbol) =>
                                {
                                    feedList = feeder.GetFeedList(symbol.Id, 1, 10);      // Get the list of values for a given symbolId of a market for given time-span
                                    sender.SendFeed(feedList);
                                });
            }
        }
    }
}