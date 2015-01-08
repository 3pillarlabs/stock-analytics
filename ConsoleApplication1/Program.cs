using FeederInterface.Sender;
using StockModel.Master;
using StockServices.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            
            ISender sender = SenderFactory.GetSender(FeederQueueSystem.REDIS_CACHE);
            sender.SendFeed(null);
        }
    }
}
