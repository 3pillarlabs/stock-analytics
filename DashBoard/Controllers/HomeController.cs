using DashBoard.Models;
using StackExchange.Redis;
using StockModel.Master;
using StockServices.Master;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DashBoard.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            TrendModel trendModel = new TrendModel();
            trendModel.SelectedExchange= Exchange.FAKE_NASDAQ;
            trendModel.SelectedSymbolId = trendModel.Symbols.Where(x => x.Value == "1").SingleOrDefault().Value;
            trendModel.SelectedSymbolVal = trendModel.Symbols.Where(x => x.Value == "1").SingleOrDefault().Text;

            return View(trendModel);
        }

        public ActionResult Search(TrendModel trendModel)
        {
            trendModel.SelectedSymbolVal = trendModel.Symbols.Where(x => x.Value == trendModel.SelectedSymbolId).SingleOrDefault().Text;
            return View("Index",trendModel);
        }
    }
}