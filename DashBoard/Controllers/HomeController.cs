using StackExchange.Redis;
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
        IDatabase cache = RedisCacheConfig.GetCache();
        public ActionResult Index()
        {
            List<string[]> list = new List<string[]>();
            StreamWriter writer = new StreamWriter(@"D:\testFile.txt", true);
            
            for (int i = 1; i <= 10; i++)
            {
                int symbolId = i;
                string value = cache.StringGet(symbolId.ToString() + (0).ToString());      // Gets the stockPrice for given symbolId from the RedisCache
                if (!String.IsNullOrEmpty(value))
                {
                    string[] array = new string[] { symbolId.ToString(), value };
                    list.Add(array);
                }
                writer.WriteLine(value + symbolId.ToString());
            }
            writer.Close();
            return View();
        }
    }
}