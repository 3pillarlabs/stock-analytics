using FeederInterface.Sender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServices.Sender
{
    public class RedisCacheSender : ISender
    {
        public bool SendFeed(StockModel.Feed feed)
        {
            throw new NotImplementedException();
        }
    }
}
