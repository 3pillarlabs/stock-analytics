
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
using StockModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using StockModel.Master;
using StockServices.Util;

namespace StockServices.Sender
{
    public class RedisCacheSender : ISender
    {
        IDatabase cache = RedisCacheConfig.GetCache();
        ConnectionMultiplexer connection = RedisCacheConfig.GetConnection();

        public bool SendFeed(List<StockModel.Feed> feeds)
        {
            ISubscriber sub = connection.GetSubscriber();


            Parallel.ForEach(feeds, (feed) =>
            {
                string text = Convert.ToBase64String(ObjectSerialization.SerializeToStream(feed).ToArray());
                sub.PublishAsync(Exchange.FAKE_NASDAQ.ToString(), text);
              
            });
            return true;
        }


        public ISubscriber IPublisher { get; set; }
    }
}
