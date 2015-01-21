using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockModel.Master
{
    public enum DropDownIdentifier
    {
        EXCHANGE,
        SYMBOL
    }

    public enum Exchange
    {
        NASDAQ,
        FAKE_NASDAQ
    }

    public enum FeederSourceSystem
    {
        GOOGLE,
        FAKEMARKET
    }

    public enum FeederQueueSystem
    {
        REDIS_CACHE
        
    }


}
