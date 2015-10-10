using FeederInterface.Feeder;
using StockModel.Master;
using StockServices.Feeder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServices.Factory
{
    public class FeederFactory
    {
        public static IFeeder GetFeeder(FeederSourceSystem source)
        {
            switch (source)
            {
                case FeederSourceSystem.GOOGLE:
                    {
                        return new GoogleFinanceFeeder();
                    }
                case FeederSourceSystem.FAKEMARKET:
                    {
                        return new FakeMarketFeeder();
                    }
                case FeederSourceSystem.YAHOO:
                    {
                        return new YahooFinanceFeeder();
                    }
                default:
                    {
                        return new GoogleFinanceFeeder();
                    }
            }
        }
    }
}
