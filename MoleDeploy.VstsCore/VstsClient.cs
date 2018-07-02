using System;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace MoleDeploy.Vsts {
    public class VstsClient {
        private VssConnection _Connection;

        public VstsClient(string collectionUri, string accessToken) {
            VssCredentials creds = new VssBasicCredential(string.Empty, accessToken);
            _Connection = new VssConnection(new Uri(collectionUri), creds);
        }

        public async Task<string> GetLastBuildStatus(string project, int definitionId, int releaseId)
        {
            var buildClient = _Connection.GetClient<BuildHttpClient>();
            var releaseClient = _Connection.GetClient<ReleaseHttpClient>();

            var definition = await buildClient.GetDefinitionAsync(project, definitionId, includeLatestBuilds: true);
            var latestBuild = definition.LatestBuild;
            var status = latestBuild.Status;
            var releaseDefinition = await releaseClient.GetReleaseDefinitionAsync(project, releaseId);
            var lastReleaseId = releaseDefinition.LastRelease.Id;

            var release = await releaseClient.GetReleaseAsync(project, lastReleaseId);

            return status.ToString();
        }

        public async Task SubmitNewBuild(string project, int definitionId, string parameters)
        {
            var buildClient = _Connection.GetClient<BuildHttpClient>();
            var releaseClient = _Connection.GetClient<ReleaseHttpClient>();

            var definition = await buildClient.GetDefinitionAsync(project, definitionId);

            await buildClient.QueueBuildAsync(new Build
            {
                 Definition = new DefinitionReference
                {
                    Id = definition.Id
                },
                Project = definition.Project,
                Parameters = parameters
            });
        }
        
    }
}