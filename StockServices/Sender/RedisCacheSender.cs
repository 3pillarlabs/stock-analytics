
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

using FeederInterface.Sender;
using StockServices.Master;
using System.Diagnostics;
using System.IO;

namespace StockServices.Sender
{
    public class RedisCacheSender : ISender
    {
        IDatabase cache = RedisCacheConfig.GetCache();
        StreamWriter writer = new StreamWriter(new FileStream(@"d:\test.txt", FileMode.Append, FileAccess.Write));

        public bool SendFeed(List<List<StockModel.Feed>> feedListRange)
        {
            Stopwatch stopWatch = Stopwatch.StartNew();
            List<StockModel.Feed> feed = new List<StockModel.Feed>();
            feed = feedListRange[feedListRange.Count - 1];

            for(int j = 0; j< feed.Count; j++)
            {
                cache.StringSetAsync(feed[j].SymbolId.ToString() + j.ToString(), feed[j].LTP.ToString(), TimeSpan.FromMinutes(90)); // Sets the stockPrice as value in RedisCache for symbolId as a key
                string value = cache.StringGet(feed[j].SymbolId.ToString() + j.ToString());
                writer.WriteLine(value + "-" + feed[j].SymbolId.ToString() + j.ToString());
            }
            stopWatch.Stop();
            
            writer.WriteLine(stopWatch.ElapsedMilliseconds.ToString());
            writer.Flush();
            return true;
        }
    }
}
