using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using StockModel;
using StockServices.FakeMarketService;
using StockServices.Dashboard;


namespace FakeMarket
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class FakeMarket : IFakeMarket
    {
        public Symbol symbolInfo = new Symbol();
        List<Feed[]> generatedData = new List<Feed[]>();
        List<Symbol> symbolList = new List<Symbol>();
        public FakeMarket()
        {
            if (InMemoryObjects.FakeFeeds.Count == 0)
            {
                FakeDataGenerator fakeData = new FakeDataGenerator();
            }
            
        }

        public Symbol GetPriceBySymbol(int symbolId, int exchangeId)
        {
            if (generatedData.Count != 0)
            {
                Feed data = generatedData.Last()[symbolId];

                symbolInfo = symbolList.SingleOrDefault(x => x.Id == symbolId);
                symbolInfo.DefaultVal = data.LTP;
            }
            return symbolInfo;
        }

        public List<Feed> GetPriceBySymbol(int symbolId, int exchangeId, TimeSpan lastAccessTime)
        {
            List<Feed> feedsList = new List<Feed>();
            if (generatedData.Count != 0)
            {
                List<Feed> list = generatedData.Select(x => x[symbolId - 1]).ToList();
                feedsList = list.Where(x => x.TimeStamp >= lastAccessTime.Milliseconds).ToList();
                
            }
            return feedsList;
        }

        public List<Symbol> GetPriceBySymbol(int symbolId, int exchangeId, bool getAllData)
        {
            throw new NotImplementedException();
        }

        public List<Symbol> GetPrice(int exchangeId)
        {
            throw new NotImplementedException();
        }

        public List<List<Symbol>> GetPrice(int exchangeId, TimeSpan lastAccessTime)
        {
            throw new NotImplementedException();
        }

        public List<List<Symbol>> GetPrice(int exchangeId, bool getAllData)
        {
            throw new NotImplementedException();
        }
    }
}
