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
using StockServices.Dashboard;
using StockServices.FakeMarketService;

namespace StockFeeder
{
    public partial class Feeder : ServiceBase
    {
        delegate void MethodDelegate();
        List<List<Feed>> feedList = new List<List<Feed>>();
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
            //Loading system startup data
            InMemoryObjects.LoadInMemoryObjects();

            //Initiate fake data generation from fake market
            //Later it will also include data generation from google finance
            TimeSpan updateDuration = TimeSpan.FromMilliseconds(Constants.FAKE_DATA_GENERATE_PERIOD);
            FakeDataGenerator.StartFakeDataGeneration(updateDuration);


            IFeeder feeder = FeederFactory.GetFeeder(FeederSourceSystem.FAKEMARKET);
            ISender sender = SenderFactory.GetSender(FeederQueueSystem.REDIS_CACHE);

            //For each market start generating the data and pushing it into redis cache
            for (int i = 1; i <= 10; i++)       // Get the stockValue for symbolId from 1 to 10
            {
                feedList = feeder.GetFeedList(i, 1, TimeSpan.FromSeconds(10));      // Get the list of values for a given symbolId of a market for given time-span
                sender.SendFeed(feedList);
            }
        }
    }
}