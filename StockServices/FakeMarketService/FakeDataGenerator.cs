using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockModel;
using StockModel.Master;
using StockServices.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StockServices.FakeMarketService
{
    public class FakeDataGenerator
    {
        public static Constants constant = new Constants();
        public static Random random = new Random();
        public static List<Feed[]> feedsList = new List<Feed[]>();
        public static DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public FakeDataGenerator()
        {
            TimeSpan _updateDuration = TimeSpan.FromMilliseconds(constant.fakeDataGeneratePeriod);
            GenerateData(null);
            Timer dataGenerateTimer = new Timer(GenerateData, null, TimeSpan.FromMilliseconds(0), _updateDuration);
        }

        public static void GenerateData(object state)
        {
            // Method to generate feeds and update the in memory objects
            List<StockModel.Symbol> symbols = GetAllSymbols();
            TimeSpan _updateInterval = TimeSpan.FromMilliseconds(constant.fakeDataGenerateInterval);
            UpdateData(symbols);
            Timer timer = new Timer(UpdateData, symbols, _updateInterval, _updateInterval); // Timer to update the stock-values after every given time-interval
        }

        public static List<StockModel.Symbol> GetAllSymbols()
        {
            // Method to get all the company names & their symbols and set their default value & Id
            int i = 1;
            Constants constant = new Constants();
            Random random = new Random();
            List<StockModel.Symbol> symbols = new List<StockModel.Symbol>();
            string jsonString = System.IO.File.ReadAllText(constant.symbolsFilePath);
            JArray jsonArray = JsonConvert.DeserializeObject<JArray>(jsonString);
            foreach (JObject jsonObject in jsonArray)
            {
                StockModel.Symbol symbol = new StockModel.Symbol();
                foreach (var property in jsonObject)
                {
                    if (property.Key == "Name")
                    {
                        symbol.SymbolName = property.Value.ToString();
                    }
                    if (property.Key == "Symbol")
                    {
                        symbol.SymbolCode = property.Value.ToString();
                    }
                    symbol.DefaultVal = random.NextDouble() * 1000;
                    symbol.Id = i;
                }
                i = i + 1;
                symbols.Add(symbol);
            }
            InMemoryObjects.ExchangeSymbolList.Symbols = symbols;
            InMemoryObjects.ExchangeSymbolList.Exchange = Exchange.FAKE_NASDAQ;
            return symbols;
        }

        public static void UpdateData(object state)
        {
            // Method to change the values of all the stocks randomly in a fixed range 
            List<StockModel.Symbol> symbols = (List<StockModel.Symbol>)state;
            Feed[] feedsArray = new Feed[symbols.Count];
            
            for (int i = 0; i < symbols.Count; i++)
            {
                double changePercent = random.NextDouble() * (constant.maximumChangePercent - constant.minimumChangePercent) + constant.minimumChangePercent;
                symbols[i].DefaultVal = symbols[i].DefaultVal + symbols[i].DefaultVal * changePercent / 100;
                Feed feed = new Feed();
                feed.SymbolId = symbols[i].Id;
                feed.LTP = symbols[i].DefaultVal;
                feed.Id = i;

                feed.TimeStamp = Convert.ToInt64((DateTime.Now - epoch).TotalMilliseconds);
                feedsArray[i] = feed;
            }
            InMemoryObjects.fakeFeeds.Add(feedsArray);
        }
    }
}
