using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockInterface.Dashboard;
using System.Web.Mvc;
using StockModel.Master;
using StockDataServices.Master;
using StockServices.DashBoard;

namespace StockServices.Master
{
    public class MasterService : IMasterService
    {


        public IEnumerable<SelectListItem> GetDropDownData(DropDownRequest request)
        {
            IEnumerable<SelectListItem> dropDownData = null;
            switch (request.Identifier)
            {
                case DropDownIdentifier.EXCHANGE:
                    {
                        dropDownData = MasterDataService.GetExchangeList(request);
                        break;
                    }
                case DropDownIdentifier.SYMBOL:
                    {
                        dropDownData = InMemoryObjects.ExchangeSymbolList.SingleOrDefault(x => x.Exchange == Exchange.FAKE_NASDAQ).Symbols.Select(x => new SelectListItem()
                        {
                            Text = x.SymbolName.ToString(),
                            Value = ((int)x.Id).ToString()
                        }).ToList();

                        break;
                    }
                default:
                    {
                        dropDownData = null;
                        break;
                    }

            }

            return dropDownData;
        }
    }
}
