using StockModel.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockModel
{
    public class ExchangeSymbol
    {
        public Exchange Exchange { get; set; }

        public List<Symbol> Symbols {get; set;}
    }
}
