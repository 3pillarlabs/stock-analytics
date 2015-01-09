
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
        StreamWriter writer = new StreamWriter(new FileStream(@"c:\test.txt", FileMode.Append));

        public bool SendFeed(List<List<StockModel.Feed>> feedListRange)
        {

            Stopwatch stopWatch = Stopwatch.StartNew();
            List<StockModel.Feed> feed = new List<StockModel.Feed>();
            feed = feedListRange[feedListRange.Count - 1];

            Parallel.For(0, 1000, i =>
            {
                cache.StringSetAsync("key21=" + i.ToString(), feed[feed.Count - 1].LTP.ToString(), TimeSpan.FromMinutes(15));
                writer.WriteLine(cache.StringGet("key21=" + i.ToString()));
            });

            stopWatch.Stop();
            
            writer.WriteLine(stopWatch.ElapsedMilliseconds.ToString());
            
            
            return true;
        }

        public void send(int i)
        {
            cache.StringSetAsync("key21=" + i.ToString(), "key21value=" + i.ToString(), TimeSpan.FromMinutes(15));
            writer.WriteLine(cache.StringGet("key21=" + i.ToString()));
        }
    }
}
