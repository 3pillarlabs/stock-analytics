using StockInterface.Feeder;
using StockInterface.DataProcessing;
using StockModel;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.Generic;

namespace StockServices.Aggregators
{
    public class RXProcessing
    {
        static IObservable<double> eventAsObservable;
        static IDisposable subs;

        public static void AddAggregator(IDataPublisher publisher, 
            IAggregator<double, double> agg, 
            Action<double, int> act, int symbolId)
        {
            eventAsObservable = from update in  Observable.FromEvent<OnFeedReceived, Feed>(
                mktH => publisher.FeedArrived += mktH,
                mktH => publisher.FeedArrived -= mktH
                ).Where((feed) => feed.SymbolId == symbolId)
                                    select update.LTP;

            //aggregates and yields results.
            subs = eventAsObservable
                .Scan<double, double>(0,
                (acc, currentValue) =>
                {
                    return agg.Aggregate(currentValue);
                }).Subscribe((state) => act(state, symbolId));

        }
    }
}
