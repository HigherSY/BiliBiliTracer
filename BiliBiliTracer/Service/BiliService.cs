using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using BiliBiliTracer.Model;

namespace BiliBiliTracer.Service
{
    public class BiliService
    {
        private readonly IConfigurationSection _cfg;
        private readonly Random _rnd;
        private readonly IHttpClientFactory _factory;

        public BiliService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _rnd = new Random();
            _cfg = configuration.GetSection("BiliService");
            _factory = httpClientFactory;
        }

        public HttpClient newClient()
        {
            //var endpoints = _cfg.GetSection("Endpoints").Get<string[]>();
            //int p = _rnd.Next(endpoints.Length);

            HttpClient client = _factory.CreateClient();
            //client.BaseAddress = new Uri($"https://{endpoints[p]}/");
            //client.DefaultRequestHeaders.Host = "api.bilibili.com";
            client.BaseAddress = new Uri("https://api.bilibili.com/");
            client.DefaultRequestHeaders.Add("Referer", _cfg["Referer"]);
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9,en;q=0.8,en-GB;q=0.7,en-US;q=0.6");
            client.DefaultRequestHeaders.Add("User-Agent", _cfg["UserAgent"]);

            return client;
        }

        public async Task<T> GetJsonData<T>(string requestUri)
        {
            var client = newClient();
            var response = await client.GetFromJsonAsync<BiliData<T>>(requestUri);
            if (response.code == 0)
            {
                return response.data;
            }
            else
            {
                throw new BiliApiException($"{requestUri} returned {response.code}: {response.message}");
            }
        }

        public async Task<JsonElement> GetJsonData(string requestUri) => await GetJsonData<JsonElement>(requestUri);
    }
}
