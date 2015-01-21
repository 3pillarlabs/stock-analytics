using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using StockInterface.DashBoardDataServices;
using StockModel.Master;
using StockDataServices.Master;


namespace StockDataServices.Master
{
    public class MasterDataService : IMasterDataService
    {
        public static IEnumerable<SelectListItem> GetExchangeList(DropDownRequest request)
        {
            IEnumerable<SelectListItem> exchangeList = null;
            exchangeList = Enum.GetValues(typeof(Exchange)).Cast<Exchange>().Select(x => new SelectListItem()
            {
                Text = x.ToString(),
                Value = ((int)x).ToString()
            }).ToList();

            return exchangeList;
        }

        public IEnumerable<SelectListItem> GetDropDownData(DropDownRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
