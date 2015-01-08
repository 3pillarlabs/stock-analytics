using FeederInterface.Sender;
using StockModel.Master;
using StockServices.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServices.Factory
{
    public class SenderFactory
    {

        public static ISender GetSender(FeederQueueSystem queueSystem)
        {
            switch (queueSystem)
            {
                case FeederQueueSystem.REDIS_CACHE:
                    {
                        return new RedisCacheSender();
                    }
                
                default:
                    {
                        return new RedisCacheSender();
                    }
            }
        }
    }
}
