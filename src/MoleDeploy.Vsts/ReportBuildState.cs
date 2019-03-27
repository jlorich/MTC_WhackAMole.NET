using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MoleDeploy.Contracts;
using Microsoft.Extensions.Configuration;

namespace MoleDeploy.Vsts.Functions
{
    public static class ReportBuildState
    {
        [FunctionName("ReportBuildState")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request,
            [SignalR(HubName="MoleDeploy")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            var body = new StreamReader(request.Body).ReadToEnd();
            var notification = JsonConvert.DeserializeObject<VstsBuildStateChangeNotification>(body);

            await signalRMessages.AddAsync(
                new SignalRMessage {
                    Target = "StatusChanged",
                    Arguments = new[] { notification }
                }
            );

            return new OkResult();
        }
    }
}
