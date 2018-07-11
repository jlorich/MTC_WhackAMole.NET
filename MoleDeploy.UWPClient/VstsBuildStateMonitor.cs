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
        public delegate void BuildStateChangeHandler(object sender, VstsBuildState state, string message = null);

        public event BuildStateChangeHandler OnStateBegin;
        public event BuildStateChangeHandler OnStateEnd;

        private VstsBuildState state;

        private HubConnection _Connection;

        public VstsBuildStateMonitor(string endpoint, string accessKey, string hubName)
        {
            var signalR = new AzureSignalR($"Endpoint={endpoint};AccessKey={accessKey}");
            var hubUrl = signalR.GetClientHubUrl(hubName);
            var token = signalR.GenerateAccessToken(hubName);

            _Connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options => {
                options.Headers = new Dictionary<string, string> { { "Authorization", string.Format("bearer {0}", token) } };
            })
            .Build();

            _Connection.On<VstsBuildStateChangeNotification>("StatusUpdate", (noticifation) =>
            {
                ProcessStateChangeNotification(noticifation);
            });
        }

        public Task ConnectToHubAsync()
        {
            return _Connection.StartAsync();
        }

        private void ProcessStateChangeNotification(VstsBuildStateChangeNotification notification) {
            if (state != VstsBuildState.Unknown && state != VstsBuildState.DeployComplete && notification.State != state)
            {
                OnStateEnd(this, state);
            }

            if (notification?.State == null)
            {
                return;
            }

            state = notification.State;

            OnStateBegin(this, notification.State, notification.Message);
        }
    }
}
