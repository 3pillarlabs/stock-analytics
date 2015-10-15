using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using DashBoard.Hubs;
using System;
using System.Collections.Concurrent;
using System.Web.Mvc;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using System.Drawing;
using System.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using SignalRChart.Web.Models;
using StockServices.Factory;
using StockModel.Master;
using FeederInterface.Feeder;
using StockServices.DashBoard;
using FeederInterface.Sender;
using StockServices.FakeMarketService;
using System.Threading.Tasks;
using StackExchange.Redis;
using StockServices.Master;
using StockModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using StockServices.Util;

namespace DashBoard.Controllers
{
    public class ChartController : Controller
    {
        // Singleton instance
        private readonly static Lazy<ChartController> _instance = new Lazy<ChartController>(() => new ChartController(GlobalHost.ConnectionManager.GetHubContext<ChartHub>().Clients));

        private readonly ConcurrentQueue<string> _stocks = new ConcurrentQueue<string>();
        private readonly TimeSpan _updateInterval = TimeSpan.FromSeconds(10);

        private volatile bool _updatingData = false;
        private readonly object _updateDataLock = new object();

        static List<StockModel.Feed> feedList = new List<StockModel.Feed>();

        delegate void MethodDelegate();

        ConnectionMultiplexer connection = RedisCacheConfig.GetConnection();

        public string GroupIdentifier = "1";
        public string SelectedExchange = "FAKE_NASDAQ";

        private void start()
        {
            byte[] binary = null;
            MemoryStream stream = null;

            ISubscriber sub = connection.GetSubscriber();
            Feed feed = null;

            sub.Subscribe(SelectedExchange, (channel, message) =>
            {
                string str = message;
                binary = Convert.FromBase64String(message);
                stream = new MemoryStream(binary);
                feed = (Feed)ObjectSerialization.DeserializeFromStream(stream);
                double[] stockData = new double[2];
                stockData[0] = feed.TimeStamp;
                stockData[1] = feed.LTP;

                if (stockData != null && stockData.Length != 0)
                {
                    Clients.Group(feed.SymbolId.ToString()).updatePoints(stockData[0], stockData[1]);
                }

            });

            while (true)
            {
                Thread.Sleep(600000);
            }
        }

        public ChartController(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;
            MethodDelegate metho = new MethodDelegate(this.start);
            metho.BeginInvoke(null, null);
        }

        public IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        public static ChartController Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public void UpdateData(object state)
        {
            //GetStockValue("NSE", "ABB", DateTime.Now);
            lock (_updateDataLock)
            {
                if (!_updatingData)
                {
                    _updatingData = true;

                    try
                    {
                        //string response = new WebClient().DownloadString("https://www.google.com/finance/getprices?q=ABB&x=NSE&i=60&p=1m&f=d,c,v,o,h,l");
                        //string[] parsedResponse = ParseResponse(response);
                        UpdateGraph();
                    }
                    catch (Exception ex)
                    {
                        System.IO.File.AppendAllText(@"D:\exception.txt", ex.Message);
                    }

                    _updatingData = false;
                }
            }
        }

        public string[] ParseResponse(string response)
        {
            string[] res = Regex.Split(response, "\n");
            int count = res.Length;
            if (count >= 9)
            {
                string[] values = Regex.Split(res[7], ",");
                string[] stockData = { values[0], values[1] };
                return stockData;
            }
            else
            {
                return null;
            }
        }

        public void UpdateGraph()
        {
            IFeeder feeder = FeederFactory.GetFeeder(FeederSourceSystem.FAKEMARKET);

            feedList = feeder.GetFeedList(1, 1, 10);      // Get the list of values for a given symbolId of a market for given time-span

            for (int i = 0; i < feedList.Count; i++)
            {
                double[] stockData = new double[2];
                stockData[0] = feedList[i].TimeStamp;
                stockData[1] = feedList[i].LTP;

                if (stockData != null && stockData.Length != 0)
                {
                    //double seconds = Convert.ToDouble(stockData[0].Remove(0, 1)) * 1000;
                    Clients.All.updatePoints(stockData[0], stockData[1]);
                }
            }
        }

        public DateTime GetTime(string seconds)
        {
            seconds = seconds.Remove(0, 1);
            double secondsPassedGMT = Convert.ToDouble(seconds);
            double secondsPassedIST = secondsPassedGMT + 330 * 60;
            DateTime refTime = new DateTime(1970, 1, 1, 0, 0, 0);
            DateTime currentTime = refTime.AddSeconds(secondsPassedIST);
            return currentTime;
        }

        [HttpGet]
        public string[] GetInitialData()
        {
            string[] data = System.IO.File.ReadAllLines(@"C:\Users\sparsh.khandelwal\Desktop\data.txt");
            return data;
        }

        public void GetStockValue(string exchange, string symbol, DateTime timestamp)
        {
            string url = GetUrl("NASDAQ", "USD");
            List<Company> symbols = GetCompaniesSymbols(url);
            string jsonString = JsonConvert.SerializeObject(symbols);
            //System.IO.File.WriteAllText(@"C:\Users\sparsh.khandelwal\Documents\Visual Studio 2013\Projects\Stocks\StockAnalytics\Data\Symbol.json", jsonString);
        }

        public string GetUrl(string exchange, string currency)
        {
            string defaultUrl = String.Format("https://www.google.com/finance?output=json&start=0&num=20&noIL=1&q=[currency%20%3D%3D%20%22USD%22%20%26%20%28exchange%20%3D%3D%20%22NASDAQ%22%29%20%26%20%28market_cap%20%3E%3D%200%29%20%26%20%28market_cap%20%3C%3D%204920000000000%29]&restype=company&ei=rQCtVMnHCobrkwXsxYG4CA");
            string url = defaultUrl.Replace("USD", currency);
            url = url.Replace("NASDAQ", exchange);
            string response = new WebClient().DownloadString(url);
            int number_Companies = GetNumberOfCompanies(response);
            string replaceString = String.Format("num={0}", number_Companies);
            url = url.Replace("num=20", replaceString);

            return url;
        }

        public int GetNumberOfCompanies(string response)
        {
            int numberOfCompanies = 0;
            response = response.Replace(@"\", "");
            JObject parsedResponse = JsonConvert.DeserializeObject<JObject>(response);
            foreach (var property in parsedResponse)
            {
                string key = property.Key;
                if (key == "num_company_results")
                {
                    numberOfCompanies = Convert.ToInt16(property.Value.ToString());
                }
            }
            return numberOfCompanies;
        }

        public List<Company> GetCompaniesSymbols(string url)
        {
            int i = 0;
            List<Company> companies = new List<Company>();
            string response = new WebClient().DownloadString(url);
            response = response.Replace(@"\", "");
            JObject parsedResponse = JsonConvert.DeserializeObject<JObject>(response);
            foreach (var property in parsedResponse)
            {
                string key = property.Key;
                if (key == "searchresults")
                {
                    JArray values = (JArray)property.Value;
                    foreach (JObject property2 in values)
                    {
                        Company company = new Company();
                        foreach (var property3 in property2)
                        {
                            if (property3.Key == "title")
                            {
                                company.Name = property3.Value.ToString();
                            }
                            else if (property3.Key == "ticker")
                            {
                                company.Symbol = property3.Value.ToString();
                            }
                        }
                        companies.Add(company);
                    }
                }
            }

            return companies;
        }

    }
}