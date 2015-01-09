﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockModel;

namespace FeederInterface.Feeder
{
    public interface IFeeder
    {
        List<Feed> GetFeed(int symbolId, int exchangeId);
        List<List<Feed>> GetFeedList(int symbolId, int exchangeId, TimeSpan lastAccessTime);
    }
}
