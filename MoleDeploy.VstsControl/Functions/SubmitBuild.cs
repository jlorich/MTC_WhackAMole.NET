using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using System.Configuration;

namespace MoleDeploy.Vsts.Functions
{
    public static class SubmitNewBuild
    {
        private static string VstsAccessToken {
            get
            {
                return ConfigurationManager.AppSettings["VstsAccessToken"];
            }
        }

        private static string CollectionUri
        {
            get
            {
                return ConfigurationManager.AppSettings["VstsCollection"];
            }
        }

        private static VstsClient Client
        {
            get
            {
                return new VstsClient(CollectionUri, VstsAccessToken);
            }
        }

        [FunctionName("SubmitBuild")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            var request = await req.Content.ReadAsAsync<SubmitBuildRequest>();

            var color = request.Color;
            var project = ConfigurationManager.AppSettings["VstsCollection"];
            var definitionId = int.Parse(ConfigurationManager.AppSettings["VstsCollection"]);
            var args = $"--color={color}";
            
            await Client.SubmitNewBuild(project, definitionId, args);

            return new HttpResponseMessage(HttpStatusCode.Created);
        }
    }
}
