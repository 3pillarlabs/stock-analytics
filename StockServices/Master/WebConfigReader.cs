using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockServices.Master
{
    public static class WebConfigReader
    {
        public static string Read(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (ConfigurationManager.AppSettings[key] == null)
                {
                    throw new NullReferenceException("The key/value must be defined in the app config for key: " + key);
                }
                else
                {
                    return ConfigurationManager.AppSettings[key].ToString();
                }
            }
            else
            {
                throw new ArgumentException("Must pass a value to read settings from web.config");
            }
        }
    }
}
