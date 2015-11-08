
using FeederInterface.Sender;
using StackExchange.Redis;
using StockServices.Master;
using StockServices.Util;
using System;
using StockModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StockServices.Sender
{
    public class RedisCacheSender : ISender
    {
        IDatabase cache = RedisCacheConfig.GetCache();
        ConnectionMultiplexer connection = RedisCacheConfig.GetConnection();

        /// <summary>
        /// Send a list of feeds to redis
        /// </summary>
        /// <param name="feeds"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public bool SendFeed(List<Feed> feeds, string exchange)
        {
            ISubscriber sub = connection.GetSubscriber();

            Parallel.ForEach(feeds, (feed) =>
            {
                string text = Convert.ToBase64String(ObjectSerialization.SerializeToStream(feed).ToArray());
                sub.PublishAsync(exchange, text);
              
            });
            return true;
        }

        /// <summary>
        /// Send a single feed to redis
        /// </summary>
        /// <param name="feed"></param>
        /// <param name="exchange"></param>
        /// <returns></returns>
        public bool SendFeed(Feed feed, string exchange)
        {
            ISubscriber sub = connection.GetSubscriber();

            string text = Convert.ToBase64String(ObjectSerialization.SerializeToStream(feed).ToArray());
            sub.PublishAsync(exchange, text);

            return true;
        }

        public bool SendMVA(double mva, string symbolid)
        {
            ISubscriber sub = connection.GetSubscriber();
            
            sub.PublishAsync(symbolid, mva.ToString());

            return true;
        }
        public ISubscriber IPublisher { get; set; }
    }
}
