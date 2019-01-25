using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace MVC2013.Src.Comun.Util
{
    public static class FormatUtil
    {


        /*public static string getCurrencyString(decimal? amount) {
            if (amount != null && amount.HasValue) { 
                try{
                    return String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:#,0.00}", amount.GetValueOrDefault(0));
                }catch {
                    return String.Empty;
                }
            }
            return String.Empty;
        }*/

        public static string getCurrencyString(decimal amount)
        {
            return String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:#,0.00}", amount);
        }

        public static string getCurrencyString(decimal? amount)
        {
            if (!amount.HasValue)
            {
                return String.Empty;
            }
            return String.Format(System.Globalization.CultureInfo.InvariantCulture, "{0:#,0.00}", amount.GetValueOrDefault(0));
        }

        public static decimal? getDecimalFromString(string amountStr) { 
            if(string.IsNullOrEmpty(amountStr))
               return null;
            try{
                return Decimal.Parse(amountStr, NumberStyles.Currency, CultureInfo.InvariantCulture);
            }catch {
                return null;
            }
            
        }


    }
}