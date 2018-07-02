using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Configuration;

namespace MoleDeploy.Vsts.Functions
{
    public static class SubmitNewBuild
    {
        [FunctionName("SubmitBuild")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log, ExecutionContext context)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var request = await req.Content.ReadAsAsync<SubmitBuildRequest>();

            var color = request.Color;
            var collection = config["Vsts:Collection"];
            var projectName = config["Vsts:ProjectName"];
            var definitionId = int.Parse(config["Vsts:BuildId"]);
            var accessToken = config["Vsts:AccessToken"];
            var parameters = $"pod_color={color}";

            var client = new VstsClient(collection, accessToken);

            await client.SubmitNewBuild(projectName, definitionId, parameters);

            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}
