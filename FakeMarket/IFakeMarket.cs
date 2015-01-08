using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using StockModel;

namespace FakeMarket
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IFakeMarket
    {

        [OperationContract]
        Symbol GetPriceBySymbol(int symbolId, int exchangeId);

        [OperationContract]
        Symbol GetPriceBySymbol(int symbolId, int exchangeId, TimeSpan lastAccessTime);

        [OperationContract]
        Symbol GetPriceBySymbol(int symbolId, int exchangeId, Boolean getAllData);


        [OperationContract]
        List<Symbol> GetPrice(int exchangeId);

        [OperationContract]
        List<Symbol> GetPrice(int exchangeId, TimeSpan lastAccessTime);

        [OperationContract]
        List<Symbol> GetPrice(int exchangeId, Boolean getAllData);
        
    }


    
}
