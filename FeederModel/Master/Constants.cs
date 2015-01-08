using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockModel.Master
{
    public class Constants
    {
        public double maximumChangePercent { get { return 2.0; } }
        public double minimumChangePercent { get { return -2.0; } }
        public string symbolsFilePath { get { return @"C:\Symbol.json"; } }
        public int fakeDataGenerateInterval { get { return 1000; } }
        public int fakeDataGeneratePeriod { get { return 1000 * 60 * 10; } }
    }
}
