using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using MoleDeploy.Contracts;
using MoleDeploy.SignalR;
using Newtonsoft.Json;

namespace MoleDeploy.Vsts
{
    public static class ReportBuildState
    {
        private const string endpoint = "https://mtcden-sandbox-demo-whack-a-mole.service.signalr.net";// ApplicationData.Current.LocalSettings.Values["moleServiceEndpoint"] as string;
        private const string accessKey = "HBCBkIRl/CqVBMbG9VUzrQ4Cp9msXAVBKVPeCzpEkR0=";// ApplicationData.Current.LocalSettings.Values["moleServiceEndpoint"] as string;

        [FunctionName("ReportBuildState")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            var body = await req.Content.ReadAsStringAsync();
            var notification = JsonConvert.DeserializeObject<VstsBuildStateChangeNotification>(body);

            var signalR = new AzureSignalR($"Endpoint={endpoint};AccessKey={accessKey}");

            var result = await signalR.SendAsync("Status", "StatusChanged", notification);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
