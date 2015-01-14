using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockModel
{
    public class SymbolFeeds
    {
        public int SymbolId { get; set; }

        public List<Feed> Feeds { get; set; }
    }
}
