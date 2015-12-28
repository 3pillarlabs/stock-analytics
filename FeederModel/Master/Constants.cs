using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockModel.Master
{
    public static class Constants
    {
        public const double  MAX_CHANGE_PERC =2.0;

        public const double MIN_CHANGE_PERC=-2.0;
        
        public const int FAKE_DATA_GENERATE_INTERVAL =100;
        
        public const int FAKE_DATA_GENERATE_PERIOD =1000 * 60 * 1;

        public const string REDIS_MVA_ROOM_PREFIX = "MVA_";
    }
}
