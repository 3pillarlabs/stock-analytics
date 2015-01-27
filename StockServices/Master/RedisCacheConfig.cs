using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace StockServices.Master
{
    public static class RedisCacheConfig
    {
        private static ConnectionMultiplexer connection = null;

        static RedisCacheConfig()
        {
            string isLocal = WebConfigReader.Read("IsLocal");

            if (isLocal == "1")
            {
                connection = ConnectionMultiplexer.Connect(WebConfigReader.Read("RedisServer"));
            }
            else
            {
                var options = new ConfigurationOptions();

                options.EndPoints.Add(WebConfigReader.Read("RedisKeyDns"), 6380);
                options.Ssl = true;

                options.Password = WebConfigReader.Read("RedisPassword");
                options.AllowAdmin = true;

                // necessary?
                options.KeepAlive = 30;
                options.ConnectTimeout = 15000;
                options.SyncTimeout = 15000;

                connection = ConnectionMultiplexer.Connect(options);
            }

        }

        public static IDatabase GetCache()
        {
            IDatabase cache = connection.GetDatabase();
            return cache;
        }

        public static ConnectionMultiplexer GetConnection()
        {
            return connection;
        }
    }
}
