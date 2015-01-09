using FeederInterface.Feeder;
using StockModel;
using StockServices.Dashboard;
using StockServices.FakeMarketService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockServices.Feeder
{
    public class FakeMarketFeeder:IFeeder
    {
        public StockModel.Symbol symbolInfo = new StockModel.Symbol();
        List<Feed[]> generatedData = new List<Feed[]>();
        List<StockModel.Symbol> symbolList = new List<StockModel.Symbol>();
        
        public FakeMarketFeeder()
        {
            if (InMemoryObjects.fakeFeeds.Count == 0)
            {
                FakeDataGenerator fakeData = new FakeDataGenerator();
            }
            generatedData = InMemoryObjects.fakeFeeds;
            symbolList = InMemoryObjects.ExchangeSymbolList.Symbols;
        }

        public List<Feed> GetFeed(int symbolId, int exchangeId)
        {
            if (InMemoryObjects.fakeFeeds.Count == 0)
            {
                FakeDataGenerator fakeData = new FakeDataGenerator();
            }
            generatedData = InMemoryObjects.fakeFeeds;
            symbolList = InMemoryObjects.ExchangeSymbolList.Symbols;
            List<Feed> feedsList = new List<Feed>();
            Parallel.For(1, 100, i =>
            {
                Feed feedList = GetPriceBySymbol(symbolId, exchangeId);
                feedsList.Add(feedList);
            });
            return feedsList;
        }

        public List<List<Feed>> GetFeedList(int symbolId, int exchangeId, TimeSpan lastAccessTime) 
        {
            if (InMemoryObjects.fakeFeeds.Count == 0)
            {
                FakeDataGenerator fakeData = new FakeDataGenerator();
            }
            generatedData = InMemoryObjects.fakeFeeds;
            symbolList = InMemoryObjects.ExchangeSymbolList.Symbols;
            List<List<Feed>> feedListrange = new List<List<Feed>>();
            Parallel.For(1, 100, i =>
            {
                List<Feed> feedsList = GetPriceBySymbol(symbolId, exchangeId, lastAccessTime);
                feedListrange.Add(feedsList);
            });
            return feedListrange;
        }

        public Feed GetPriceBySymbol(int symbolId, int exchangeId)
        {
            Feed data = new Feed();
            if (generatedData.Count != 0)
            {
                data = generatedData.Last()[symbolId];
            }
            return data;
        }

        public List<Feed> GetPriceBySymbol(int symbolId, int exchangeId, TimeSpan lastAccessTime)
        {
            List<Feed> feedsList = new List<Feed>();
            if (generatedData.Count != 0)
            {
                var list = generatedData.Select(x => x[symbolId]).ToList();
                feedsList = list.Where(x => x.TimeStamp >= lastAccessTime.Milliseconds).ToList();
            }
            return feedsList;
        }
    }
}
