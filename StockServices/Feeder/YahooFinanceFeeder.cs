using FeederInterface.Feeder;
using StockModel;
using StockModel.Master;
using StockServices.DashBoard;
using StockServices.PollingYahooMarketService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockServices.Feeder
{
    public class YahooFinanceFeeder : IFeeder
    {
        public StockModel.Symbol symbolInfo = new StockModel.Symbol();

        List<SymbolFeeds> generatedData = new List<SymbolFeeds>();
        List<StockModel.Symbol> symbolList = new List<StockModel.Symbol>();

        public int DeleteFeedList(int symbolId, int exchangeId, long deleteListFrom, long deleteListTo)
        {
            int i = -1;
            lock (YahooDataGenerator.thisLock)
            {
                generatedData = InMemoryObjects.ExchangeFakeFeeds.Where(x => x.ExchangeId == exchangeId).SingleOrDefault().ExchangeSymbolFeed;
                i = generatedData.Where(x => x.SymbolId == symbolId).SingleOrDefault().Feeds.RemoveAll(x => x.TimeStamp >= deleteListFrom && x.TimeStamp <= deleteListTo);
            }
            return i;
        }

        public List<Feed> GetFeedList(int symbolId, int exchangeId, long lastAccessTime)
        {
            List<Feed> feedsList = null;

            Exchange exchange = (Exchange)exchangeId;

            symbolList = InMemoryObjects.ExchangeSymbolList.SingleOrDefault(x => x.Exchange == exchange).Symbols;

            lock (YahooDataGenerator.thisLock)
            {
                generatedData = InMemoryObjects.ExchangeFakeFeeds.Where(x => x.ExchangeId == exchangeId).SingleOrDefault().ExchangeSymbolFeed;
                feedsList = generatedData.Where(x => x.SymbolId == symbolId).SingleOrDefault().Feeds.Where(x => x.TimeStamp >= lastAccessTime).ToList();
            }
            return feedsList;
        }
       
    }
}
