using StackExchange.Redis;
using StockServices.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionMultiplexer connection = RedisCacheConfig.GetConnection();
            ISubscriber sub = connection.GetSubscriber();

            sub.PublishAsync("messages", "First message from publisher1");

            Console.ReadKey();

            sub.PublishAsync("messages", "Second message from publisher1");

            Console.ReadKey();

            sub.PublishAsync("messages", "Third message from publisher1");

            Console.ReadKey();

           // ConnectionMultiplexer connection = RedisCacheConfig.GetConnection();
           // IDatabase sub = connection.GetDatabase();

           // sub.StringSet("messages", "First message from publisher1");

           //// Console.ReadKey();

           // string str=sub.StringGet("messages");

           // Console.ReadKey();

           // sub.Publish("messages", "Third message from publisher1");

           // Console.ReadKey();



        }
    }
}
