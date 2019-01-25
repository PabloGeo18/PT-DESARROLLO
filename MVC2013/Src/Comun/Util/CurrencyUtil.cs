using MVC2013.Src.Comun.View;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MVC2013.Src.Comun.Util
{
    public class CurrencyUtil
    {
        public static decimal Convert(decimal amount, string inputCurrency, string outputCurrency) 
        {
            string tasaString = ConfiguracionDb.GetSingle("Conversion_" + inputCurrency + outputCurrency);
            decimal tasa;

            if (tasaString != null)
            {
                tasa = Decimal.Parse(tasaString, CultureInfo.InvariantCulture);
                return amount * tasa;
            }
            else
            {
                tasaString = ConfiguracionDb.GetSingle("Conversion_" + outputCurrency + inputCurrency);

                if (tasaString != null)
                {
                    tasa = Decimal.Parse(tasaString, CultureInfo.InvariantCulture);
                    return amount / tasa;
                }
            }

            return amount;
        }
    }

    public static class Currencies {
        public const string Quetzal = "Quetzal";
        public const string Dolar = "Dolar";
        public const string Euro = "Euro";
    }
}