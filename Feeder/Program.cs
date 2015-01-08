using FeederInterface.Feeder;
using StockServices.Factory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feeder
{
    class Program
    {
        static void Main(string[] args)
        {
            //Read the values from calling program
            //If already running
            IFeeder feeder = FeederFactory.GetFeeder(StockModel.Master.FeedSourceSystem.FakeMarket);
        }
    }
}
