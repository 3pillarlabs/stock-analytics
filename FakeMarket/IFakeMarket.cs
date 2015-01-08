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

        [OperationContract(Name = "GetPriceBySymbolCurrent")]
        Symbol GetPriceBySymbol(int symbolId, int exchangeId);

        [OperationContract(Name = "GetPriceBySymbolTimeRange")]
        List<Feed> GetPriceBySymbol(int symbolId, int exchangeId, TimeSpan lastAccessTime);

        [OperationContract(Name = "GetPriceBySymbolAll")]
        List<Symbol> GetPriceBySymbol(int symbolId, int exchangeId, Boolean getAllData);


        [OperationContract(Name = "GetPriceForSymbolsCurrent")]
        List<Symbol> GetPrice(int exchangeId);

        [OperationContract(Name = "GetPriceForSymbolsTimeRange")]
        List<List<Symbol>> GetPrice(int exchangeId, TimeSpan lastAccessTime);

        [OperationContract(Name = "GetPriceForSymbolsAll")]
        List<List<Symbol>> GetPrice(int exchangeId, Boolean getAllData);

    }



}
