using FeederInterface.Feeder;
using StockModel;
using StockModel.Master;
using StockServices.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using FeederInterface.Sender;

namespace FakeDataConsoleApp
{
    class Program
    {
        static int i;
        static void Main(string[] args)
        {
            i = 0;
            List<List<Feed>> feedList = new List<List<Feed>>();
            IFeeder feeder = FeederFactory.GetFeeder(FeederSourceSystem.FAKEMARKET);
            feedList = feeder.GetFeedList(2, 1, new TimeSpan(0, 0, 0));
            ISender sender = SenderFactory.GetSender(FeederQueueSystem.REDIS_CACHE);
            sender.SendFeed(feedList);
            foreach (var feeds in feedList)
            {
                try
                {
                    foreach (var feed in feeds)
                    {
                        string obj = String.Format("{0} - {1}", feed.SymbolId, feed.LTP);
                        Console.WriteLine(obj);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            Console.ReadKey();
        }
    }
}
