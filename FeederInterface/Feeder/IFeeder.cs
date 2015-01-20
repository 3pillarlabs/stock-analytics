using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockModel;

namespace FeederInterface.Feeder
{
    public interface IFeeder
    {
        List<Feed> GetFeed(int symbolId, int exchangeId);
        List<Feed> GetFeedList(int symbolId, int exchangeId, long lastAccessTime);

        int DeleteFeedList(int symbolId, int exchangeId, long deleteListFrom, long deleteListTo);

    }
}
