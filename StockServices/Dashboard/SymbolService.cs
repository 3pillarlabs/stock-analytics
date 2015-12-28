using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockModel;
using StockModel.Master;
using StockServices.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace StockServices.DashBoard
{
    public class SymbolService
    {
        public static List<ExchangeSymbol> GetSymbols(List<Exchange> exchanges)
        {
            List<ExchangeSymbol> exchangeSymbols = new List<ExchangeSymbol>();

            foreach (Exchange exchange in exchanges)
            {
                ExchangeSymbol exchangeSymbol = new ExchangeSymbol();

                exchangeSymbol.Exchange = exchange;

                exchangeSymbol.Symbols = GetSymbolForMarket(exchange); ;

                exchangeSymbols.Add(exchangeSymbol);
            }

            return exchangeSymbols;

        }

        public static List<StockModel.Symbol> GetSymbolForMarket(Exchange exchange)
        {
            // Method to get all the company names & their symbols and set their default value & Id
            int i = 1;

            Random random = new Random();
            List<StockModel.Symbol> symbols = new List<StockModel.Symbol>();
            string jsonString;
            string symbolFilePath = string.Empty;
            
            //exchange specific symbol file paths are configured
            symbolFilePath = WebConfigurationManager.AppSettings[exchange.ToString() + "_SymbolFilePath"];

            //exchange specific symbol file paths are not configured. Pick Defaults.
            if (string.IsNullOrEmpty(symbolFilePath))
            {
                symbolFilePath = WebConfigReader.Read("SymbolFilePath");
            }

            jsonString = System.IO.File.ReadAllText(symbolFilePath);

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

            return symbols;
        }

        public static Boolean UpdateInMemoryObjects(List<Exchange> exchange)
        {
            return true;
        }

    }
}
