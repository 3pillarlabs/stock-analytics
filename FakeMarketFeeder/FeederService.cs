using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeederInterface.Feeder;
using FeederModel;

namespace FakeMarketFeeder
{
    public class FeederService : IFeeder
    {
        public List<Feed> GetFeed(int SymbolId)
        {
            throw new NotImplementedException();
        }
    }
}
