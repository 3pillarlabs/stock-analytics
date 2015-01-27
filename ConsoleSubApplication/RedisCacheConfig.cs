﻿using System;
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
        public static ConnectionMultiplexer connection = null;

        static RedisCacheConfig()
        {
            //var options = new ConfigurationOptions();

            //options.EndPoints.Add(ConfigurationManager.AppSettings["RedisKeyDns"], 6380);
            //options.Ssl = true;

            //options.Password = ConfigurationManager.AppSettings["RedisPassword"];
            //options.AllowAdmin = true;

            //// necessary?
            //options.KeepAlive = 30;
            //options.ConnectTimeout = 15000;
            //options.SyncTimeout = 15000;

            connection = ConnectionMultiplexer.Connect("localhost:6379");
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
