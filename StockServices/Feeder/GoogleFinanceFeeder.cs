using FeederInterface.Feeder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServices.Feeder
{
    public class GoogleFinanceFeeder : IFeeder
    {
        public List<StockModel.Feed> GetFeed(int symbolId, int exchangeId)
        {
            throw new NotImplementedException();
        }

        public List<StockModel.Feed> GetFeedList(int symbolId, int exchangeId, long lastAccessTime)
        {
            throw new NotImplementedException();
        }


        public int DeleteFeedList(int symbolId, int exchangeId, long deleteListFrom, long deleteListTo)
        {
            throw new NotImplementedException();
        }
    }
}
