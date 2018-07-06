using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Client;
using MoleDeploy.Contracts;
using Newtonsoft.Json;
using MoleDeploy.SignalR;
using System.Threading.Tasks;

namespace MoleDeploy.UWPClient
{

    public class VstsBuildStateMonitor
    {
        public delegate void BuildStateChangeHandler(object sender, VstsBuildState state);

        public event BuildStateChangeHandler OnStateBegin;
        public event BuildStateChangeHandler OnStateEnd;

        private VstsBuildState state;

        private HubConnection _Connection;

        public VstsBuildStateMonitor(string endpoint, string accessKey)
        {
            var signalR = new AzureSignalR($"Endpoint={endpoint};AccessKey={accessKey}");
            var hubUrl = signalR.GetClientHubUrl("Status");
            var token = signalR.GenerateAccessToken("Status");

            _Connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options => {
                options.Headers = new Dictionary<string, string> { { "Authorization", string.Format("bearer {0}", token) } };
            })
            .Build();

            _Connection.On<VstsBuildStateChangeNotification>("StatusChanged", (noticifation) =>
            {
                ProcessStateChangeNotification(noticifation);
            });
        }

        public Task ConnectToHubAsync()
        {
            return _Connection.StartAsync();
        }

        private void ProcessStateChangeNotification(VstsBuildStateChangeNotification message) {
            if (state != VstsBuildState.Unknown && state != VstsBuildState.DeployComplete)
            {
                OnStateEnd(this, state);
            }

            if (message?.State == null)
            {
                return;
            }

            state = message.State;

            OnStateBegin(this, message.State);
        }
    }
}
