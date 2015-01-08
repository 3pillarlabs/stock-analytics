using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using StockFeeder;
using FeederInterface.Sender;
using StockServices.Factory;
using StockModel.Master;

namespace StockFeeder
{
    public partial class Feeder : ServiceBase
    {
        delegate void MethodDelegate();

        public Feeder()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            MethodDelegate metho = new MethodDelegate(this.start);
            metho.BeginInvoke(null,null);
        }
        protected override void OnStop()
        {
        }

        private void start()
        {
            ISender sender = SenderFactory.GetSender(FeederQueueSystem.REDIS_CACHE);
            sender.SendFeed(null);

        }
    }
}
