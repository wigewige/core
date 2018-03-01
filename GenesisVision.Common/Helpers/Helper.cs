using GenesisVision.DataModel.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GenesisVision.Common.Helpers
{
    public static class Helper
    {
        public static string GetDescription(Type type)
        {
            var descriptions = (DescriptionAttribute[])type.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return descriptions.Length == 0 ? null : descriptions[0].Description;
        }

        /// <summary>
        /// Returns enum WalletCurrency. Throw Exception if currency does not exists.
        /// </summary>
        public static Currency ToCurrency(this string currency)
        {
            foreach (Currency x in Enum.GetValues(typeof(Currency)))
            {
                if (string.Equals(x.ToString(), currency, StringComparison.InvariantCultureIgnoreCase))
                    return x;
            }

            throw new KeyNotFoundException($"Could not find currency {currency}");
        }

        /// <summary>
        /// Returns enum WalletCurrency. Returns WalletCurrency.Undefined if currency does not exists.
        /// </summary>
        public static Currency ToCurrencySafe(this string currency)
        {
            foreach (Currency x in Enum.GetValues(typeof(Currency)))
            {
                if (string.Equals(x.ToString(), currency, StringComparison.InvariantCultureIgnoreCase))
                    return x;
            }

            return Currency.Undefined;
        }

        public static List<string> GetAllCurrencies()
        {
            var result = new List<string>();
            foreach (Currency x in Enum.GetValues(typeof(Currency)))
            {
                if (x == Currency.Undefined)
                    continue;

                result.Add(x.ToString());
            }
            return result;
        }
    }
}
