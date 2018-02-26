using GenesisVision.DataModel.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GenesisVision.Core.Helpers
{
    public static class Helper
    {
        public static string GetDescription(Type type)
        {
            var descriptions = (DescriptionAttribute[])type.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return descriptions.Length == 0 ? null : descriptions[0].Description;
        }

        public static WalletCurrency ToCurrency(this string currency)
        {
            foreach (WalletCurrency x in Enum.GetValues(typeof(WalletCurrency)))
            {
                if (string.Equals(x.ToString(), currency, StringComparison.CurrentCultureIgnoreCase))
                    return x;
            }

            throw new KeyNotFoundException($"Could not find currency {currency}");
        }
    }
}
