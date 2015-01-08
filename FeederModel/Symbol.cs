using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace StockModel
{
    [DataContract]
    public class Symbol
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string SymbolName {get; set;}

        [DataMember]
        public string SymbolCode { get; set; }

        [DataMember]
        public double DefaultVal { get; set; }
    }
}
