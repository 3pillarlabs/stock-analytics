using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockModel
{
    public class Feed
    {
        public long Id { get; set; }
 
        public int SymbolId { get; set; }

        public double LTP { get; set; }

        public double Open { get; set; }

        public double Close { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public double Volume { get; set; }
 
 
    }
}
