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
            // Method to get the latest stockValue for given symbolId and exchange market
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
            // Method to get the stockValue for given symbolId and exchange market for given time-span
            if (InMemoryObjects.fakeFeeds.Count == 0)
            {
                FakeDataGenerator fakeData = new FakeDataGenerator();
            }
            
            symbolList = InMemoryObjects.ExchangeSymbolList.Symbols;
            List<List<Feed>> feedListrange = new List<List<Feed>>();
            //Parallel.For(1, 100, i =>
            //{
            //    List<Feed> feedsList = GetPriceBySymbol(symbolId, exchangeId, lastAccessTime);
            //    feedListrange.Add(feedsList);
            //});
            for(int i =1; i < 100; i++)
            {
                generatedData = InMemoryObjects.fakeFeeds;
                List<Feed> feedsList = GetPriceBySymbol(symbolId, exchangeId, lastAccessTime + TimeSpan.FromSeconds(i));
                feedListrange.Add(feedsList);
                System.Threading.Thread.Sleep(300);
            };
            return feedListrange;
        }

        public Feed GetPriceBySymbol(int symbolId, int exchangeId)
        {
            // Method to get the latest stockValue for given symbolId and exchange market
            Feed data = new Feed();
            if (generatedData.Count != 0)
            {
                data = generatedData.Last()[symbolId - 1];
            }
            return data;
        }

        public List<Feed> GetPriceBySymbol(int symbolId, int exchangeId, TimeSpan lastAccessTime)
        {
            // Method to get the stockValue for given symbolId and exchange market for given time-span
            List<Feed> feedsList = new List<Feed>();
            if (generatedData.Count != 0)
            {
                var list = generatedData.Select(x => x[symbolId - 1]).ToList();
                feedsList = list.Where(x => x.TimeStamp >= lastAccessTime.Milliseconds).ToList();
            }
            return feedsList;
        }
    }
}
