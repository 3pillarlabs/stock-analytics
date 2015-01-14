using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StockModel;
using StockModel.Master;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServices.Dashboard
{
    public class SymbolService
    {
        public static List<ExchangeSymbol> GetSymbols(List<Exchange> exchanges)
        {
            List<ExchangeSymbol> exchangeSymbols = new List<ExchangeSymbol>();

            foreach (Exchange exchange in exchanges)
            {
                List<Symbol> symbols = new List<Symbol>();
                ExchangeSymbol exchangeSymbol = new ExchangeSymbol();
                exchangeSymbol.Exchange = exchange;

                switch (exchange)
                {
                    case Exchange.FAKE_NASDAQ:
                        {
                            symbols = GetSymbolForFakeMarket();
                            break;
                        }
                    case Exchange.NASDAQ:
                        {
                            symbols = GetSymbolForFakeMarket();
                            break;
                        }
                    default:
                        {
                            symbols = GetSymbolForFakeMarket();
                            break;
                        }
                }
                
                exchangeSymbol.Symbols = symbols;

                exchangeSymbols.Add(exchangeSymbol);
            }

            return exchangeSymbols;

        }

        private static List<StockModel.Symbol> GetSymbolForFakeMarket()
        {
            // Method to get all the company names & their symbols and set their default value & Id
            int i = 1;
            
            Random random = new Random();
            List<StockModel.Symbol> symbols = new List<StockModel.Symbol>();
            string jsonString = System.IO.File.ReadAllText(Constants.SYMBOL_FILE_PATH);
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
