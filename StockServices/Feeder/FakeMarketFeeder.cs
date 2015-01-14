using FeederInterface.Feeder;
using StockModel;
using StockModel.Master;
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
    public class FakeMarketFeeder : IFeeder
    {
        public StockModel.Symbol symbolInfo = new StockModel.Symbol();

        List<SymbolFeeds> generatedData = new List<SymbolFeeds>();
        List<StockModel.Symbol> symbolList = new List<StockModel.Symbol>();

        public FakeMarketFeeder()
        {
            if (InMemoryObjects.FakeFeeds.Count == 0)
            {
                FakeDataGenerator fakeData = new FakeDataGenerator();
            }
        }


        public List<Feed> GetFeed(int symbolId, int exchangeId)
        {
            // Method to get the latest stockValue for given symbolId and exchange market
            if (InMemoryObjects.FakeFeeds.Count == 0)
            {
                FakeDataGenerator fakeData = new FakeDataGenerator();
            }
            generatedData = InMemoryObjects.ExchangeFakeFeeds.Where(x => x.ExchangeId == Convert.ToInt32(Exchange.FAKE_NASDAQ)).SingleOrDefault().ExchaneSymbolFeed;
            symbolList = InMemoryObjects.ExchangeSymbolList.SingleOrDefault(x => x.Exchange == Exchange.FAKE_NASDAQ).Symbols;

            List<Feed> feedsList = new List<Feed>();
            Parallel.For(1, 100, i =>
            {
                Feed feedList = GetPriceBySymbol(symbolId, exchangeId, generatedData);
                feedsList.Add(feedList);
            });
            return feedsList;
        }

        public List<List<Feed>> GetFeedList(int symbolId, int exchangeId, TimeSpan lastAccessTime)
        {
         
            symbolList = InMemoryObjects.ExchangeSymbolList.SingleOrDefault(x => x.Exchange == Exchange.FAKE_NASDAQ).Symbols;

            List<List<Feed>> feedListrange = new List<List<Feed>>();
            //Parallel.For(1, 100, i =>
            //{
            //    List<Feed> feedsList = GetPriceBySymbol(symbolId, exchangeId, lastAccessTime);
            //    feedListrange.Add(feedsList);
            //});
            for (int i = 1; i < 100; i++)
            {
                generatedData = InMemoryObjects.ExchangeFakeFeeds.Where(x => x.ExchangeId == Convert.ToInt32(Exchange.FAKE_NASDAQ)).SingleOrDefault().ExchaneSymbolFeed;
                List<Feed> feedsList = GetPriceBySymbol(symbolId, exchangeId, lastAccessTime + TimeSpan.FromSeconds(i), generatedData);
                feedListrange.Add(feedsList);
                System.Threading.Thread.Sleep(300);
            };
            return feedListrange;
        }

        private Feed GetPriceBySymbol(int symbolId, int exchangeId, List<SymbolFeeds> generatedData)
        {
            // Method to get the latest stockValue for given symbolId and exchange market
            Feed data  = generatedData.Where(x => x.SymbolId == symbolId).SingleOrDefault().Feeds.Last();
            return data;
        }

        private List<Feed> GetPriceBySymbol(int symbolId, int exchangeId, TimeSpan lastAccessTime, List<SymbolFeeds> generatedData)
        {
            // Method to get the stockValue for given symbolId and exchange market for given time-span
            List<Feed> feedsList = new List<Feed>();
            feedsList = generatedData.Where(x => x.SymbolId == symbolId).SingleOrDefault().Feeds.Where(x => x.TimeStamp >= lastAccessTime.Milliseconds).ToList();
            return feedsList;
        }
    }
}
