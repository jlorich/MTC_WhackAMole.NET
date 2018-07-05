using System.IO;
using System.Net.Http;

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
        private readonly KubeSettings _settings;

        public LocalServiceTokenProvider(KubeSettings settings)
        {
            _settings = settings;
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
            if (_settings.AccessToken != null)
                return _settings.AccessToken;

            if (!File.Exists(_settings.AccessTokenPath))
                throw new FileNotFoundException();

            var token = File.ReadAllText(_settings.AccessTokenPath);

            return token;
        }
    }
}