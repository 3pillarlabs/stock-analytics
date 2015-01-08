using FeederInterface.Feeder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServices.Feeder
{
    public class FakeMarketFeeder:IFeeder
    {
        public List<StockModel.Feed> GetFeed(int symbolId, int exchangeId)
        {
            throw new NotImplementedException();
        }
    }
}
