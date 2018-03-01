using GenesisVision.Common.Helpers;
using GenesisVision.Common.Models;
using GenesisVision.Common.Services.Interfaces;
using GenesisVision.DataModel.Enums;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GenesisVision.Common.Services
{
    public class RateService : IRateService
    {
        private readonly HttpClient client;
        private readonly IMemoryCache memoryCache;
        private const string CacheKey = "ExchangeRates";
        private readonly string requestUrl;

        public RateService(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            client = new HttpClient();
            requestUrl = string.Format("https://min-api.cryptocompare.com/data/pricemulti?fsyms={0}&tsyms={0}",
                string.Join(",", Helper.GetAllCurrencies()));
        }

        public async Task<decimal> GetRateAsync(Currency from, Currency to)
        {
            if (from == Currency.Undefined || to == Currency.Undefined)
                throw new Exception("Wrong currency");

            if (!memoryCache.TryGetValue(CacheKey, out Dictionary<string, decimal> exchangeRates))
            {
                exchangeRates = await GetExchangeRates();
                memoryCache.Set(CacheKey, exchangeRates,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(60)));
            }

            if (exchangeRates.TryGetValue($"{from}/{to}", out var value))
                return value;

            throw new Exception($"Can't get rate {from}/{to}");
        }

        public OperationResult<decimal> GetRate(Currency from, Currency to)
        {
            return InvokeOperations.InvokeOperation(() =>
            {
                var rate = GetRateAsync(from, to).Result;
                return rate;
            });
        }

        private async Task<Dictionary<string, decimal>> GetExchangeRates()
        {
            using (var httpRequest = new HttpRequestMessage(HttpMethod.Get, requestUrl))
            using (var response = await client.SendAsync(httpRequest).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsStringAsync();

                var rs = JObject.Parse(result);
                if (rs == null)
                    throw new Exception();

                var data = new Dictionary<string, decimal>();
                foreach (var item in rs.Children())
                {
                    if (item is JProperty property)
                    {
                        foreach (var jToken in property.Value)
                        {
                            var jproperty = (JProperty)jToken;
                            data.Add($"{property.Name}/{jproperty.Name}", jproperty.Value.Value<decimal>());
                        }
                    }
                }

                return data;
            }
        }
    }
}
