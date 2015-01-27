using StackExchange.Redis;
using StockModel;
using StockServices.Master;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSubApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionMultiplexer connection = RedisCacheConfig.GetConnection();
            ISubscriber sub = connection.GetSubscriber();

            byte[] binary = null;
            MemoryStream stream = null;

            Feed feed = null;

            sub.Subscribe("FAKE_NASDAQ", (channel, message) =>
            {
                string str = message;
                binary = Convert.FromBase64String(message);
                stream = new MemoryStream(binary);
                
            });

  

            Console.ReadLine();

        }

        public static MemoryStream SerializeToStream(object o)
        {
            MemoryStream stream = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, o);
            return stream;
        }

        public static object DeserializeFromStream(MemoryStream stream)
        {
            IFormatter formatter = new BinaryFormatter();
            stream.Seek(0, SeekOrigin.Begin);
            object o = formatter.Deserialize(stream);
            return o;
        }
    }
}
