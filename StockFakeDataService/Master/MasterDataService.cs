using StockInterface.DashboardDataServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockModel.Master;

namespace StockFakeDataService.Master
{
    public class MasterDataService : IMasterDataService
    {
        private static List<DropDownData> dropDownData = new List<DropDownData>();
        public MasterDataService()
        {
            //Keep adding fake list of symbols here
        }

        public List<DropDownData> GetDropDownData(StockModel.Master.DropDownRequest request)
        {
            
            switch (request.Identifier)
            {
                case DropDownIdentifier.SYMBOL:
                    {
                        return dropDownData;
                    }
                default:
                    {
                        return dropDownData;
                    }
            }
        }
    }
}
