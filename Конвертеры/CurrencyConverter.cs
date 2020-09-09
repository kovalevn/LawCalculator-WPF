using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using System.Windows;
using System.ComponentModel;
using System.Data;
using System.Threading.Tasks;

namespace LawCalculator_WPF
{
    static class CurrencyConverter
    {
        public static string rawResponseString;
        public static Dictionary<string, Currency> jsonResult;

        public static void Initialize()
        {
            using (WebClient client = new WebClient())
            {
                rawResponseString = client.DownloadString("http://www.floatrates.com/daily/rub.json");
                jsonResult = JsonConvert.DeserializeObject<Dictionary<string, Currency>>(rawResponseString);
            }
        }

        public static double ConvertToRouble(double amountInCurrency, CurrencyType currency)
        {
            switch (currency)
            {
                case CurrencyType.Dollar:
                case CurrencyType.DollarCashless:
                    return amountInCurrency * jsonResult["usd"].inverseRate;
                case CurrencyType.Euro:
                case CurrencyType.EuroCashless:
                    return amountInCurrency * jsonResult["eur"].inverseRate;
                default:
                    return amountInCurrency;
            }
        }
    }
}
