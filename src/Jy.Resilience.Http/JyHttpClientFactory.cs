using System;
using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Jy.Resilience.Http
{
    public class JyHttpClientFactory
    {
        private static IServiceProvider _serviceProvider;

        public static void Init(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        //using (var client = JapxHttpClientFactory.CreateClient(new Uri(baseUrl)))
        public static HttpClient CreateClient(Uri baseAddress)
        {
            var factory = _serviceProvider.GetService<IHttpClientFactory>();

            var client = factory.CreateClient();
            client.BaseAddress = baseAddress;
            return client;
        }
    }
}
