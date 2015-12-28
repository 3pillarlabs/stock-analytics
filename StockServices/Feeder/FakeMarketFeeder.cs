using FeederInterface.Feeder;
using StockModel;
using StockModel.Master;
using StockServices.DashBoard;
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

        public List<Feed> GetFeedList(int symbolId, int exchangeId, long lastAccessTime)
        {
            List<Feed> feedsList = null;

            Exchange exchange = (Exchange)exchangeId;

            symbolList = InMemoryObjects.ExchangeSymbolList.SingleOrDefault(x => x.Exchange == exchange).Symbols;

            lock (FakeDataGenerator.LockDataGeneration)
            {
                generatedData = InMemoryObjects.ExchangeFakeFeeds.Where(x => x.ExchangeId == Convert.ToInt32(exchangeId)).SingleOrDefault().ExchangeSymbolFeed;
                feedsList = generatedData.Where(x => x.SymbolId == symbolId).SingleOrDefault().Feeds.Where(x => x.TimeStamp >= lastAccessTime).ToList();
            }
            return feedsList;
        }



        public int DeleteFeedList(int symbolId, int exchangeId, long deleteFrom, long deleteTo)
        {
            int i = -1;
            lock (FakeDataGenerator.LockDataGeneration)
            {
                generatedData = InMemoryObjects.ExchangeFakeFeeds.Where(x => x.ExchangeId == Convert.ToInt32(Exchange.FAKE_NASDAQ)).SingleOrDefault().ExchangeSymbolFeed;
                i = generatedData.Where(x => x.SymbolId == symbolId).SingleOrDefault().Feeds.RemoveAll(x => x.TimeStamp >= deleteFrom && x.TimeStamp <= deleteTo);
            }
            return i;
        }

        #region Commented Code

        //private Feed GetPriceBySymbol(int symbolId, int exchangeId, List<SymbolFeeds> generatedData)
        //{
        //    // Method to get the latest stockValue for given symbolId and exchange market
        //    Feed data = generatedData.Where(x => x.SymbolId == symbolId).SingleOrDefault().Feeds.Last();
        //    return data;
        //}

        //private List<Feed> GetPriceBySymbol(int symbolId, int exchangeId, long lastAccessTime, List<SymbolFeeds> generatedData)
        //{
        //    // Method to get the stockValue for given symbolId and exchange market for given time-span
        //    List<Feed> feedsList = new List<Feed>();
        //    feedsList = generatedData.Where(x => x.SymbolId == symbolId).SingleOrDefault().Feeds.Where(x => x.TimeStamp >= lastAccessTime).ToList();

        //    return feedsList;
        //}


        //public List<Feed> GetFeed(int symbolId, int exchangeId)
        //{
        //    // Method to get the latest stockValue for given symbolId and exchange market

        //    generatedData = InMemoryObjects.ExchangeFakeFeeds.Where(x => x.ExchangeId == Convert.ToInt32(Exchange.FAKE_NASDAQ)).SingleOrDefault().ExchangeSymbolFeed;
        //    symbolList = InMemoryObjects.ExchangeSymbolList.SingleOrDefault(x => x.Exchange == Exchange.FAKE_NASDAQ).Symbols;

        //    List<Feed> feedsList = new List<Feed>();
        //    Parallel.For(1, 100, i =>
        //    {
        //        Feed feedList = GetPriceBySymbol(symbolId, exchangeId, generatedData);
        //        feedsList.Add(feedList);
        //    });
        //    return feedsList;
        //}

        #endregion
    }
}
