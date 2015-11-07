using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace StockModel
{
    [Serializable]
    public class Feed: ICloneable
    {
        public long Id { get; set; }
 
        public int SymbolId { get; set; }

        public double LTP { get; set; }

        public double Open { get; set; }

        public double Close { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public double Volume { get; set; }

        public Int64 TimeStamp { get; set; }

        public object Clone()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream();
            using (stream)
            {
                formatter.Serialize(stream, this);
                stream.Seek(0, SeekOrigin.Begin);
                return (Feed)formatter.Deserialize(stream);
            }
        }
    }
}
