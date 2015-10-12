using Newtonsoft.Json;
using StockModel;
using StockModel.Master;
using StockServices.DashBoard;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace StockServices.PollingYahooMarketService
{
    public class YahooDataGenerator
    {
        public static Object thisLock = new Object();
        private static int updateDurationTime = 300;
        private delegate void UpdateDataDelegate();


        /*
        Original test URL:
        private readonly string yqlURL = "https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.quotes%20where%20symbol%20in%20(%22{0}%22)&StockExchange%20in%20(%22{1}%22)&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=";
        */

        private static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private const string baseYFinanceURL = "https://query.yahooapis.com/v1/public/yql";

        private const string yql = "select * from yahoo.finance.quotes where symbol in (%22{0}%22)&StockExchange in (%22{1}%22)";
        private const string format = "json";
        private const string env = "store://datatables.org/alltableswithkeys";
        private static Exchange exchange;

        public static void StartDataGeneration(int updateTimePeriod, Exchange exch)
        {
            updateDurationTime = updateTimePeriod;
            exchange = exch;
            UpdateDataDelegate del = new UpdateDataDelegate(GenerateData);
            del.BeginInvoke(null, null);
        }

        public static void GenerateData()
        {
            // Method to generate feeds and update the in memory objects
            List<StockModel.Symbol> symbols = InMemoryObjects.ExchangeSymbolList.SingleOrDefault
                (x => x.Exchange == exchange).Symbols;
            UpdateData(symbols);
        }


        private static void UpdateData(object state)
        {
            List<StockModel.Symbol> symbols = (List<StockModel.Symbol>)state;
            Feed[] feedsArray = new Feed[symbols.Count];

            List<SymbolFeeds> symbolFeeds = new List<SymbolFeeds>();

            while (true)
            {
                Thread.Sleep(updateDurationTime);

                Parallel.ForEach(symbols, (symbol) =>
                {
                    Feed feed = GetFeed(symbol.SymbolCode, symbol.Id, exchange.ToString());

                    //locking the static collection as it will be read from several sources, causing synchroization issues
                    lock (thisLock)
                    {
                        try
                        {
                            InMemoryObjects.ExchangeFakeFeeds.Find
                            (x => x.ExchangeId == Convert.ToInt32(exchange))
                            .ExchangeSymbolFeed.Find(x => x.SymbolId == symbol.Id).Feeds.Add(feed);
                        }
                        catch (Exception ex)
                        {
                            //TODO: Logging
                        }
                    }
                });

            }
        }

        private static Feed GetFeed(string symbol, int symbolId, string exchange)
        {

            string url = string.Format("{0}?q={1}&format={2}&env={3}", baseYFinanceURL,
                HttpUtility.UrlPathEncode(string.Format(yql, symbol, exchange)),
                HttpUtility.UrlPathEncode(format),
                Uri.EscapeDataString(HttpUtility.UrlPathEncode(env))
                );

            WebRequest request = WebRequest.Create(url);
            WebResponse response = request.GetResponse();

            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            RootObject rootData = JsonConvert.DeserializeObject<RootObject>(responseFromServer);

            Feed fd = new Feed();
            fd.SymbolId = symbolId;
            fd.Id = symbolId;
            fd.High = Convert.ToDouble(string.IsNullOrEmpty(rootData.query.results.quote.DaysHigh) ? "0" : rootData.query.results.quote.DaysHigh);
            fd.Low = Convert.ToDouble(string.IsNullOrEmpty(rootData.query.results.quote.DaysLow) ? "0" : rootData.query.results.quote.DaysHigh);
            fd.LTP = Convert.ToDouble(string.IsNullOrEmpty(rootData.query.results.quote.LastTradePriceOnly) ? "0" : rootData.query.results.quote.DaysHigh);
            fd.Open = Convert.ToDouble(string.IsNullOrEmpty(rootData.query.results.quote.Open) ? "0" : rootData.query.results.quote.DaysHigh);
            fd.TimeStamp = Convert.ToInt64((DateTime.Now - epoch).TotalMilliseconds);

            return fd;
        }

        protected class Quote
        {
            public string symbol { get; set; }
            public string Ask { get; set; }
            public string AverageDailyVolume { get; set; }
            public string Bid { get; set; }
            public object AskRealtime { get; set; }
            public object BidRealtime { get; set; }
            public string BookValue { get; set; }
            public string Change_PercentChange { get; set; }
            public string Change { get; set; }
            public object Commission { get; set; }
            public string Currency { get; set; }
            public object ChangeRealtime { get; set; }
            public object AfterHoursChangeRealtime { get; set; }
            public string DividendShare { get; set; }
            public string LastTradeDate { get; set; }
            public object TradeDate { get; set; }
            public string EarningsShare { get; set; }
            public object ErrorIndicationreturnedforsymbolchangedinvalid { get; set; }
            public string EPSEstimateCurrentYear { get; set; }
            public string EPSEstimateNextYear { get; set; }
            public string EPSEstimateNextQuarter { get; set; }
            public string DaysLow { get; set; }
            public string DaysHigh { get; set; }
            public string YearLow { get; set; }
            public string YearHigh { get; set; }
            public object HoldingsGainPercent { get; set; }
            public object AnnualizedGain { get; set; }
            public object HoldingsGain { get; set; }
            public object HoldingsGainPercentRealtime { get; set; }
            public object HoldingsGainRealtime { get; set; }
            public object MoreInfo { get; set; }
            public object OrderBookRealtime { get; set; }
            public string MarketCapitalization { get; set; }
            public object MarketCapRealtime { get; set; }
            public string EBITDA { get; set; }
            public string ChangeFromYearLow { get; set; }
            public string PercentChangeFromYearLow { get; set; }
            public object LastTradeRealtimeWithTime { get; set; }
            public object ChangePercentRealtime { get; set; }
            public string ChangeFromYearHigh { get; set; }
            public string PercebtChangeFromYearHigh { get; set; }
            public string LastTradeWithTime { get; set; }
            public string LastTradePriceOnly { get; set; }
            public object HighLimit { get; set; }
            public object LowLimit { get; set; }
            public string DaysRange { get; set; }
            public object DaysRangeRealtime { get; set; }
            public string FiftydayMovingAverage { get; set; }
            public string TwoHundreddayMovingAverage { get; set; }
            public string ChangeFromTwoHundreddayMovingAverage { get; set; }
            public string PercentChangeFromTwoHundreddayMovingAverage { get; set; }
            public string ChangeFromFiftydayMovingAverage { get; set; }
            public string PercentChangeFromFiftydayMovingAverage { get; set; }
            public string Name { get; set; }
            public object Notes { get; set; }
            public string Open { get; set; }
            public string PreviousClose { get; set; }
            public object PricePaid { get; set; }
            public string ChangeinPercent { get; set; }
            public string PriceSales { get; set; }
            public string PriceBook { get; set; }
            public string ExDividendDate { get; set; }
            public string PERatio { get; set; }
            public string DividendPayDate { get; set; }
            public object PERatioRealtime { get; set; }
            public string PEGRatio { get; set; }
            public string PriceEPSEstimateCurrentYear { get; set; }
            public string PriceEPSEstimateNextYear { get; set; }
            public string Symbol { get; set; }
            public object SharesOwned { get; set; }
            public string ShortRatio { get; set; }
            public string LastTradeTime { get; set; }
            public object TickerTrend { get; set; }
            public string OneyrTargetPrice { get; set; }
            public string Volume { get; set; }
            public object HoldingsValue { get; set; }
            public object HoldingsValueRealtime { get; set; }
            public string YearRange { get; set; }
            public object DaysValueChange { get; set; }
            public object DaysValueChangeRealtime { get; set; }
            public string StockExchange { get; set; }
            public string DividendYield { get; set; }
            public string PercentChange { get; set; }
        }

        protected class Results
        {
            public Quote quote { get; set; }
        }

        protected class Query
        {
            public int count { get; set; }
            public string created { get; set; }
            public string lang { get; set; }
            public Results results { get; set; }
        }

        protected class RootObject
        {
            public Query query { get; set; }
        }
    }
}
