using StockInterface.Dashboard;
using StockServices.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using StockModel.Master;


namespace DashBoard.Models
{
    public class TrendModel
    {

        public TrendModel()
        {
            IMasterService masterService = new MasterService();
            this.Exchange = masterService.GetDropDownData(new DropDownRequest { Identifier = DropDownIdentifier.EXCHANGE });
            this.Symbols = masterService.GetDropDownData(new DropDownRequest { Identifier = DropDownIdentifier.SYMBOL });
        }

        public IEnumerable<SelectListItem> Exchange { get; set; }

        public Exchange SelectedExchange { get; set; }

        public IEnumerable<SelectListItem> Symbols { get; set; }

        public String SelectedSymbolId { get; set; }

        public String SelectedSymbolVal { get; set; }

        
    }

    
}