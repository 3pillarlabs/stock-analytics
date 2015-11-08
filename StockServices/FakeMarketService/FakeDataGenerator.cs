using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockInterface.Feeder;
using StockModel;
using StockModel.Master;
using StockServices.DashBoard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockServices.FakeMarketService
{
    public class FakeDataGenerator:IDataPublisher
    {
        #region Public variables
        public static Object LockDataGeneration = new Object();
        public OnFeedReceived FeedArrived { get; set; }
        #endregion

        #region Private variables
        private Random random = new Random();
        private DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private int updateDurationTime = 300;
        private Dictionary<int, List<OnFeedReceived>> notifyList;

        private static object _lockSingleton = new object();
        private static object _lockSubscription = new object();
        private static FakeDataGenerator _instance;

        #endregion

        #region Singleton
        private FakeDataGenerator()
        {
            //singleton
            notifyList = new Dictionary<int, List<OnFeedReceived>>();
        }

        public static FakeDataGenerator Instance {
            get
            {
                lock(_lockSingleton)
                {
                    if(_instance == null)
                    {
                        _instance = new FakeDataGenerator();
                    }
                    return _instance;
                }
            }
        }
        #endregion

        #region IDataPublisher members
        public void StartDataGeneration(int refreshInterval, Exchange exchange)
        {
            updateDurationTime = refreshInterval;

            Task tskDataGen = Task.Run(new Action(UpdateData));
        }

        /// <summary>
        /// Method for subscribing to the feeds of a given symbol.
        /// </summary>
        /// <param name="symbolId">Symbol Id to subscribe to</param>
        /// <param name="handler">Delegate for handling feed updates</param>
        public void SubscribeFeed(int symbolId, OnFeedReceived handler)
        {
            lock(_lockSubscription)
            {
                if (notifyList.ContainsKey(symbolId))
                {
                    notifyList[symbolId].Add(handler);
                }
                else
                {
                    notifyList.Add(symbolId, new List<OnFeedReceived>() { handler });
                }
            }
        }

        /// <summary>
        /// Method for unsubscribing to stop receiving updates for a given symbol
        /// </summary>
        /// <param name="symbolId">Symbol id to unsubscribe from</param>
        /// <param name="handler">handler to remove - multiple handlers may be attached to the same symbol</param>
        public void UnsubscribeFeed(int symbolId, OnFeedReceived handler)
        {
            lock(_lockSubscription)
            {
                if (notifyList.ContainsKey(symbolId) && notifyList[symbolId].Contains(handler))
                {
                    notifyList[symbolId].Remove(handler);
                }
                else
                {
                    throw new Exception("Subscription required for unsubscribing!");
                }
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Method for generating fake data
        /// </summary>
        private void UpdateData()
        {
            // Method to change the values of all the stocks randomly in a fixed range 
            List<StockModel.Symbol> symbols = InMemoryObjects.ExchangeSymbolList.SingleOrDefault(x => x.Exchange == Exchange.FAKE_NASDAQ).Symbols;

            Feed[] feedsArray = new Feed[symbols.Count];

            List<SymbolFeeds> symbolFeeds = new List<SymbolFeeds>();

            while (true)
            {
                Thread.Sleep(updateDurationTime);

                Parallel.ForEach(symbols, (symbol) =>
                {
                    SymbolFeeds feeds = new SymbolFeeds();
                    feeds.SymbolId = symbol.Id;

                    double changePercent = random.NextDouble() * (Constants.MAX_CHANGE_PERC - Constants.MIN_CHANGE_PERC) + Constants.MIN_CHANGE_PERC;

                    symbol.DefaultVal = symbol.DefaultVal + symbol.DefaultVal * changePercent / 100;
                    Feed feed = new Feed();
                    feed.SymbolId = symbol.Id;
                    feed.LTP = symbol.DefaultVal;
             
                    feed.TimeStamp = Convert.ToInt64((DateTime.Now - epoch).TotalMilliseconds);

                    if (FeedArrived != null)
                        FeedArrived((Feed)feed.Clone());

                    //notify subscribers - later to be changed to only notify if there is any new data
                    Notify(symbol.Id, feed);
                });

            }

        }

        /// <summary>
        /// Method for notifying all subscribers that feed has arrived.
        /// </summary>
        /// <param name="symbolId">Id of the symbol for which feed has arrived</param>
        /// <param name="fd">Feed</param>
        private void Notify(int symbolId, Feed fd)
        {
            lock (_lockSubscription)
            {
                if (notifyList.ContainsKey(symbolId))
                {
                    foreach (OnFeedReceived hndl in notifyList[symbolId])
                    {
                        try
                        {
                            //send a copy
                            hndl((Feed)fd.Clone());
                        }
                        catch
                        {
                            //ignore...
                        }
                    }
                }
            }
        }
        #endregion

    }
}
