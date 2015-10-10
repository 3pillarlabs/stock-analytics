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
        FAKE_NASDAQ,
        ASX,
        NSDQ
    }

    public enum FeederSourceSystem
    {
        GOOGLE,
        FAKEMARKET,
        YAHOO
    }
    
    public enum FeederQueueSystem
    {
        REDIS_CACHE
        
    }


}
