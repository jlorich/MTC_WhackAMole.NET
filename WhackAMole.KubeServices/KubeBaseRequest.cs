using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WhackAMole.KubeServices.Interfaces;
using WhackAMole.KubeServices.Providers;
using Newtonsoft.Json;
using System.Web;

namespace WhackAMole.KubeServices
{
    public class KubeBaseRequest<T> : IKubeRequest where T : IKubeResource
    {

        const string API_VERSION = "api/v1";

        private readonly IAuthenticationProvider _auth;
        private readonly KubeSettings _settings;


        public KubeBaseRequest(IAuthenticationProvider authProvider, KubeSettings settings)
        {
            _auth = authProvider;
            _settings = settings;
        }

        protected async Task<string> GetAsync(string api, string nameSpace = "", KeyValuePair<string, string>[] queryvalues = null)
        {
            var http = _auth.GetConnection();
            var request = CreateRequest(api, nameSpace, queryvalues);
            var result = await http.GetStringAsync(request);
            return result;
        }

        protected async Task<T> GetAsync<T>(string api, string nameSpace = "", KeyValuePair<string, string>[] queryvalues = null) where T : class
        {
            var json = await GetAsync(api, queryvalues: queryvalues);
            return JsonConvert.DeserializeObject<T>(json);
        }

        protected async Task<bool> DeleteAsync(string api, string nameSpace = "")
        {
            var http = _auth.GetConnection();
            var request = CreateRequest(api, nameSpace);
            var result = await http.DeleteAsync(request);

            return result.IsSuccessStatusCode;
        }

        private string CreateRequest(string api, string nameSpace = "", KeyValuePair<string, string>[] queryvalues = null)
        {
            var request = (nameSpace == "") ? $"{_settings.BaseApiUrl}/{API_VERSION}/{api}" : $"{_settings.BaseApiUrl}/{API_VERSION}/namespaces/{nameSpace}/{api}";
            if (queryvalues == null)
                return request;
            var query = HttpUtility.ParseQueryString(string.Empty);
            foreach (var queryvalue in queryvalues)
                query[queryvalue.Key] = queryvalue.Value;

            return $"{request}?{query.ToString()}";
        }
    }
}
