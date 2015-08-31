using Microsoft.AspNet.SignalR;
using DashBoard.Controllers;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using System.Diagnostics;
using StockServices.Master;

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

        public void GetAllStocks(string symbolId)
        {
         
            if (string.IsNullOrEmpty(symbolId))
            {
                symbolId = "1";
            }
            _chartController.GroupIdentifier = symbolId;
            JoinRoom(symbolId);
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
            SignalConnectionManager.StartProcess();

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

        public static void StartProcess()
        {
            lock (lockObj)
            {
                if (SignalConnectionManager.ConnectedClient == 1)
                {
                    StockDataGeneratorProcess = new Process();
                    StockDataGeneratorProcess.StartInfo.FileName = WebConfigReader.Read("DataGeneratorProcessPath");
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