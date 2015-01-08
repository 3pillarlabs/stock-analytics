using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using StockModel;


namespace FakeMarket
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class FakeMarket : IFakeMarket
    {
        public Symbol  GetPriceBySymbol(int symbolId, int exchangeId)
        {
            throw new NotImplementedException();
        }

        public Symbol GetPriceBySymbol(int symbolId, int exchangeId, TimeSpan lastAccessTime)
        {
            throw new NotImplementedException();
        }

        public Symbol GetPriceBySymbol(int symbolId, int exchangeId, bool getAllData)
        {
            throw new NotImplementedException();
        }

        public List<Symbol> GetPrice(int exchangeId)
        {
            throw new NotImplementedException();
        }

        public List<Symbol> GetPrice(int exchangeId, TimeSpan lastAccessTime)
        {
            throw new NotImplementedException();
        }

        public List<Symbol> GetPrice(int exchangeId, bool getAllData)
        {
            throw new NotImplementedException();
        }
    }
}
