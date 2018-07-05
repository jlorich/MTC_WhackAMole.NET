using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;
using MoleDeploy.Contracts;
using MoleDeploy.SignalR;
using Newtonsoft.Json;

namespace MoleDeploy.Vsts.Functions
{
    public static class ReportBuildState
    {
        [FunctionName("ReportBuildState")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]HttpRequestMessage req, TraceWriter log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var body = await req.Content.ReadAsStringAsync();
            var notification = JsonConvert.DeserializeObject<VstsBuildStateChangeNotification>(body);

            var endpoint = config["SignalR:Endpoint"];
            var accessKey = config["SignalR:AccessKey"];

            var signalR = new AzureSignalR($"Endpoint={endpoint};AccessKey={accessKey}");

            var result = await signalR.SendAsync("Status", "StatusChanged", notification);

            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
