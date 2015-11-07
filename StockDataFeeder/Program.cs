﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FeederInterface.Feeder;
using StockModel;
using StockModel.Master;
using StockServices.Factory;
using FeederInterface.Sender;
using StockServices.DashBoard;
using StockServices.FakeMarketService;
using System.Threading;
using System.Configuration;
using StockServices.PollingYahooMarketService;
using StockInterface.Feeder;

namespace StockDataFeeder
{
    class Program
    {
        static IDataPublisher dataGenerator;
        static Exchange selectedExchange;

        /// <summary>
        /// Application arguments:
        /// arg0: Selected Exchange. Has to be enum StockModel.Master.Exchange
        /// arg1: Data generator to use. Has to be IDataPublisher.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            //Loading system startup data for all the exchanges
            List<Exchange> exchanges = new List<Exchange>();

            //defaults selected...
            selectedExchange = Exchange.FAKE_NASDAQ;
            dataGenerator = YahooDataGenerator.Instance;

            ResolveAppArgs(args);

            exchanges = Enum.GetValues(typeof(Exchange)).OfType<Exchange>().ToList();

            InMemoryObjects.LoadInMemoryObjects(exchanges);
            
            TimeSpan updateDuration = TimeSpan.FromMilliseconds(Constants.FAKE_DATA_GENERATE_PERIOD);

            //Start data generation - this will start fetching data for all symbols of current exchange
            //Later, need to change this to only subscribe to the specific symbol(s) selected.
            dataGenerator.StartDataGeneration(300, selectedExchange);

            ISender sender = SenderFactory.GetSender(FeederQueueSystem.REDIS_CACHE);

            List<StockModel.Symbol> symbols = InMemoryObjects.ExchangeSymbolList.SingleOrDefault(x => x.Exchange == selectedExchange).Symbols;

            List<SymbolFeeds> generatedData = new List<SymbolFeeds>();
            List<StockModel.Symbol> symbolList = new List<StockModel.Symbol>();

            int feedCount = 0;
            Parallel.ForEach(symbols, (symbol) =>
            {
                //subscribe
                dataGenerator.SubscribeFeed(symbol.Id, (Feed fd) => {
                    sender.SendFeed(new List<Feed>() {fd}, selectedExchange.ToString());

                    Console.WriteLine(feedCount++);
                });

            });

            Console.Read();
        }

        /// <summary>
        /// Process application args
        /// </summary>
        /// <param name="args"></param>
        /// <param name="selectedExchange"></param>
        private static void ResolveAppArgs(string[] args)
        {
            if (args != null)
            {
                if (args.Length > 0)
                {
                    if (!Enum.TryParse(args[0], out selectedExchange))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Incorrect/missing app args for exchange. Setting default exchange as FAKE_NASDAQ");
                        Console.ResetColor();
                        selectedExchange = Exchange.FAKE_NASDAQ;
                    }
                }
                if (args.Length > 1)
                {
                   switch(args[1].ToUpper())
                    {
                        case "YHOO":
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Setting data generator as YahooDataGenerator");
                            Console.ResetColor();
                            dataGenerator =  YahooDataGenerator.Instance;
                            break;
                        case "FAKE":
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("Setting data generator as FakeDataGenerator");
                            Console.ResetColor();
                            dataGenerator = FakeDataGenerator.Instance;
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Unexpected app args for data generator");
                            Console.ResetColor();
                            throw new Exception("Unexpected app args for data generator.");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("App args missing for data generator. Using defaults.");
                    Console.ResetColor();
                }
            }
        }
    }
}
