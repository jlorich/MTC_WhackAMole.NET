using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WhackAMole.UWPClient.Models;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace WhackAMole.UWPClient.Services
{
    // I don't like this but gets the job done
    public class MoleService : IMoleService
    {
        private static MoleService _instance;

        private static string _endpoint;
        private static string _serviceName;

        private const string MOLE_API = "api/mole";

        private MoleService()
        {
            throw new Exception("Don't do this");
        }
        private MoleService(string endpoint, string serviceName)
        {
            _endpoint = endpoint;
            _serviceName = serviceName;

        }

        private HttpClient CreateHttp()
        {
            var filter = new HttpBaseProtocolFilter();
            filter.MaxConnectionsPerServer = 20;
            filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.NoCache;
            return new HttpClient(filter);
        }

        public static MoleService Instance
        {
            get
            {
                if (_instance == null)
                    throw new Exception("Instance not created");
                return _instance;
            }
        }

        public static void Create(string endpoint, string serviceName)
        {
            if (_instance != null && endpoint != _endpoint)
                throw new Exception("Instance already created");

            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("endpoint cannot be null or empty");

            if (!Uri.IsWellFormedUriString(endpoint, UriKind.Absolute))
                throw new ArgumentException("Bad endpoint format");

            _instance = new MoleService(endpoint, serviceName);
        }

        public async Task<MoleState> GetStateUpdateAsync()
        {
            try
            {
                var _http = CreateHttp();
                var uri = $"{_endpoint}/{_serviceName}/{MOLE_API}";
                var mole = await _http.GetStringAsync(new Uri(uri));
                var state = JsonConvert.DeserializeObject<MoleState>(mole);
                return state;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ERROR: GetStateUpdateAsync: {ex.Message}");
                return null;
            }
        }


    }
}
