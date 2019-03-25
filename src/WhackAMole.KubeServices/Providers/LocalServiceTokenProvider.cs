using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Options;

namespace WhackAMole.KubeServices.Providers
{
    public abstract class BaseAuthenticationProvider
    {
        protected HttpClient CreateBaseConnection()
        {
            var handler = new HttpClientHandler();

            var http = new HttpClient(handler);
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            http.DefaultRequestHeaders.Add("Access-Control-Allow-Origin", "*");
            return http;
        }
    }

    public class LocalServiceTokenProvider : BaseAuthenticationProvider, IAuthenticationProvider
    {
        private readonly string _token;
        private readonly KubeOptions _options;

        public LocalServiceTokenProvider(IOptions<KubeOptions> options)
        {
            _options = options.Value;
            _token = GetToken();
        }

        public HttpClient GetConnection()
        {
            var http = CreateBaseConnection();
            http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_token}");

            return http;
        }

        private string GetToken()
        {
            if (_options.AccessToken != null)
                return _options.AccessToken;

            if (!File.Exists(_options.AccessTokenPath))
                throw new FileNotFoundException();

            var token = File.ReadAllText(_options.AccessTokenPath);

            return token;
        }
    }
}