using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockModel;


namespace FeederInterface.Sender
{
    public interface ISender
    {
        Boolean SendFeed(List<Feed> feed, string exchange);

        Boolean SendFeed(Feed feed, string exchange);

        Boolean SendMVA(double mva, string symbolid);
    }

}
