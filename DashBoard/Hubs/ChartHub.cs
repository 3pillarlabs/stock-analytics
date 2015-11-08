using Microsoft.AspNet.SignalR;
using DashBoard.Controllers;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using System.Diagnostics;
using StockServices.Master;
using StockModel.Master;

namespace DashBoard.Hubs
{
    //[HubName("chartHub")]
    public class ChartHub : Hub
    {
        private readonly ChartController _chartController;
        
        public ChartHub() : this(ChartController.Instance) { }

        public ChartHub(ChartController chartController)
        {
            _chartController = chartController;
        }

        public void GetAllStocks(string symbolId, string selectedExchange)
        {         
            if (string.IsNullOrEmpty(symbolId))
            {
                symbolId = "1";
            }
            if (string.IsNullOrEmpty(selectedExchange))
            {
                selectedExchange = "1";
            }

            string identifier = string.Format("{0}_{1}", symbolId, selectedExchange);
            _chartController.GroupIdentifier = identifier;
            _chartController.SelectedExchange = selectedExchange;
            _chartController.SelectedSymbolId = symbolId;
            JoinRoom(identifier);
            //return the symbols and stock-exchanges' names to the client (web page)
        }

        public Task JoinRoom(string roomName)
        {
            return Groups.Add(Context.ConnectionId, roomName);
        }

        public Task LeaveRoom(string roomName)
        {
            return Groups.Remove(Context.ConnectionId, roomName);
        }

        public override Task OnConnected()
        {
            SignalConnectionManager.AddClient();
            
            SignalConnectionManager.StartProcess(Context.QueryString["SelectedExchange"]);

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            SignalConnectionManager.RemoveClient();
            SignalConnectionManager.StopProcess();


            return base.OnDisconnected(stopCalled);
        }
    }

    public static class SignalConnectionManager
    {
        public static object lockObj= new object();
        public static int ConnectedClient { get; set; }

        public static Process StockDataGeneratorProcess { get; set; }
        

        public static void AddClient()
        {
            ConnectedClient += 1;

        }

        public static void RemoveClient()
        {
            ConnectedClient -= 1;
        }

        public static void StartProcess(string exchange)
        {
            lock (lockObj)
            {
                if (SignalConnectionManager.ConnectedClient == 1)
                {
                    StockDataGeneratorProcess = new Process();
                    StockDataGeneratorProcess.StartInfo.FileName = WebConfigReader.Read("DataGeneratorProcessPath");
                    StockDataGeneratorProcess.StartInfo.Arguments = string.Format("\"{0}\" \"{1}\"", exchange, WebConfigReader.Read("DataGenerator"));
                    StockDataGeneratorProcess.Start();              
                }
            }


        }

        public static void StopProcess()
        {
            lock (lockObj)
            {
                if (SignalConnectionManager.ConnectedClient == 0)
                {
                    StockDataGeneratorProcess.Kill();
                }
            }
        }
    }

    public static class RedisCacheManager
    {
        public static Process RedisCacheServerProcess { get; set; }

        public static object lockObj = new object();

        public static void StartProcess()
        {
            lock (lockObj)
            {
                if (SignalConnectionManager.ConnectedClient == 1)
                {
                  
                    RedisCacheServerProcess = new Process();
                    RedisCacheServerProcess.StartInfo.FileName = WebConfigReader.Read("RedisServerExePath");
                    RedisCacheServerProcess.Start();

                }
            }


        }

        public static void StopProcess()
        {
            lock (lockObj)
            {
                if (SignalConnectionManager.ConnectedClient == 0)
                {

                    RedisCacheServerProcess.Kill();
                }
            }
        }


    }
}