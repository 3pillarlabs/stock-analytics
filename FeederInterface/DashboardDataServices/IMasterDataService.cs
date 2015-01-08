﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockModel.Master;

namespace StockInterface.DashboardDataServices
{
    public interface IMasterDataService
    {
        List<DropDownData> GetDropDownData(DropDownRequest request);
    }
}