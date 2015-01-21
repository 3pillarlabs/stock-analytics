using StockModel.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace StockInterface.Dashboard
{
    public interface IMasterService
    {
        IEnumerable<SelectListItem> GetDropDownData(DropDownRequest request);
    }
}
