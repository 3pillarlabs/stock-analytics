using FeederInterface.Sender;
using StackExchange.Redis;
using StockModel;
using StockModel.Master;
using StockServices.Factory;
using StockServices.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace LTPMovingAverage
{
    class Program
    {
        private static string _channelName = "";
        private static string _exchange = "";
        private static Dictionary<string, Queue<double>> _LTPStack;
        private const int n = 10;
        private static object _lockQueue = new object();
        private static Dictionary<string, double> _sum;

        /// <summary>
        /// args:
        /// {0} - channel name - redis channel to publish to.
        /// {1} - exchange - name of the exchange for which data is fetched for MVA to be calculated. Used while publishing MVA.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args == null || args.Count() < 2)
                throw new Exception("Invalid number of app args");


            _channelName = args[0];
            _exchange = args[1];

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Aggregator started with channel name set to {0}, exchange: {1}", _channelName, _exchange);

            Start();

            Console.Read();
        }

        private static void Start()
        {
            _LTPStack = new Dictionary<string, Queue<double>>();
            ISender sender = SenderFactory.GetSender(FeederQueueSystem.REDIS_CACHE);
            _sum = new Dictionary<string, double>();

            ConnectionMultiplexer connection = null;
            connection = GetRedisConnection();

            ISubscriber sub = connection.GetSubscriber();
            Feed feed = null;

            byte[] binary = null;
            MemoryStream stream = null;

            sub.Subscribe(_exchange, (channel, message) =>
            {
                lock (_lockQueue)
                {
                    string str = message;
                    binary = Convert.FromBase64String(message);
                    stream = new MemoryStream(binary);
                    feed = (Feed)ObjectSerialization.DeserializeFromStream(stream);

                    string currentSymbolId = feed.SymbolId.ToString();

                    if (_sum.ContainsKey(currentSymbolId))
                        _sum[currentSymbolId] += feed.LTP;
                    else
                        _sum.Add(currentSymbolId, feed.LTP);

                    if (_LTPStack.ContainsKey(currentSymbolId))
                    {
                        _LTPStack[currentSymbolId].Enqueue(feed.LTP);
                    }
                    else
                    {
                        Queue<double> q = new Queue<double>();
                        q.Enqueue(feed.LTP);
                        _LTPStack.Add(currentSymbolId, q);
                    }

                    if (_LTPStack[currentSymbolId].Count > n)
                    {
                        _sum[currentSymbolId] -= _LTPStack[currentSymbolId].Dequeue();
                    }

                    //Publish MVA
                    double mva = _sum[currentSymbolId] / _LTPStack[currentSymbolId].Count();
                    sender.SendMVA(mva, _channelName + currentSymbolId);

                    Console.WriteLine("Symbol id {0} published aggregate {1} to channel {2}",
                        feed.SymbolId, mva, _channelName + currentSymbolId);
                }
            });
        }

        private static ConnectionMultiplexer GetRedisConnection()
        {
            ConnectionMultiplexer connection;
            string isLocal = ConfigurationManager.AppSettings["IsLocal"];

            if (isLocal == "1")
            {
                connection = ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["RedisServer"]);
            }
            else
            {
                var options = new ConfigurationOptions();

                options.EndPoints.Add(ConfigurationManager.AppSettings["RedisKeyDns"], 6380);
                options.Ssl = true;

                options.Password = ConfigurationManager.AppSettings["RedisPassword"];
                options.AllowAdmin = true;

                // necessary?
                options.KeepAlive = 30;
                options.ConnectTimeout = 15000;
                options.SyncTimeout = 15000;

                connection = ConnectionMultiplexer.Connect(options);
            }

            return connection;
        }
    }
}
