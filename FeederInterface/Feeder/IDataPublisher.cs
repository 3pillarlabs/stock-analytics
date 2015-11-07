﻿using StockModel;
using StockModel.Master;

namespace StockInterface.Feeder
{
    public delegate void OnFeedReceived(Feed fd);

    public interface IDataPublisher
    {
        void StartDataGeneration(int refreshInterval, Exchange exchange);

        void SubscribeFeed(int symbolId, OnFeedReceived handler);

        void UnsubscribeFeed(int symbolId, OnFeedReceived handler);
    }
}
