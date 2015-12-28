using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockModel.Master
{
    public class DropDownData
    {
        public string Key { get; set; }

        public string Value { get; set; }

    }

    public class DropDownRequest
    {
        public DropDownIdentifier Identifier { get; set; }

        public Exchange Exchange { get; set; }
    }

}
