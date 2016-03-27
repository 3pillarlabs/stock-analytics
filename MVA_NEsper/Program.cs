using com.espertech.esper.client;
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

namespace MVA_NEsper
{
    class Program
    {
        private static string _channelName = "";
        private static string _exchange = "";

        private static EPRuntime _runtime;

        private static MessageHandler mHandler;

        private static com.espertech.esper.client.Configuration nesperConf;
        private static EPServiceProvider epService;
        private static ISender sender;
        private static object _lockQueue = new object();

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
            InitializeNesper();
            WireEPLStatements();

            SubscribeRedis();
        }

        private static void SubscribeRedis()
        {
            ConnectionMultiplexer connection = null;
            connection = GetRedisConnection();
            ISubscriber sub = connection.GetSubscriber();
            sub.Subscribe(_exchange, (channel, message) =>
            {
                lock (_lockQueue)
                {
                    byte[] binary = Convert.FromBase64String(message);
                    MemoryStream stream = new MemoryStream(binary);
                    Feed feed = (Feed)ObjectSerialization.DeserializeFromStream(stream);

                    //send to NEsper
                    _runtime.SendEvent(feed);
                }
            });
            sender = SenderFactory.GetSender(FeederQueueSystem.REDIS_CACHE);
        }

        private static void WireEPLStatements()
        {
            string stmnt = @"select Avg(LTP), SymbolId
                      from Feed().win:time(10 minutes)
                      group by SymbolId";

            EPStatement statement = epService.EPAdministrator.CreateEPL(stmnt);
            statement.Subscriber = mHandler;
        }

        private static void InitializeNesper()
        {
            nesperConf = new com.espertech.esper.client.Configuration();
            nesperConf.AddEventType(typeof(Feed));

            epService = EPServiceProviderManager.GetDefaultProvider(nesperConf);
            _runtime = epService.EPRuntime;
            mHandler = new MessageHandler();
            mHandler.UpdatePrediction += Predict;
        }


        private static void Predict(double mva, int currentSymbolId)
        {
            sender.SendMVA(mva, _channelName + currentSymbolId);

            Console.WriteLine("Symbol id {0} published aggregate {1} to channel {2}",
                currentSymbolId, mva, _channelName + currentSymbolId);
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
