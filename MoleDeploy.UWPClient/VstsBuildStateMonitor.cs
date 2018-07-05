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

        private readonly string _Endpoint;
        private readonly string _AccessKey;

        private VstsBuildState state;

        public VstsBuildStateMonitor(string endpoint, string accessKey)
        {
            _Endpoint = endpoint;
            _AccessKey = accessKey;
        }

        public async Task ConnectToHubAsync()
        {
            var signalR = new AzureSignalR($"Endpoint={_Endpoint};AccessKey={_AccessKey}");
            var hubUrl = signalR.GetClientHubUrl("Status");
            var token = signalR.GenerateAccessToken("Status");

            var connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options => {
                options.Headers = new Dictionary<string, string> { { "Authorization", string.Format("bearer {0}", token) } };
            })
            .Build();

            connection.On<VstsBuildStateChangeNotification>("StatusChanged", (noticifation) =>
            {
                ProcessStateChangeNotification(noticifation);
            });

            await connection.StartAsync();
        }

        private void ProcessStateChangeNotification(VstsBuildStateChangeNotification message) {
            if (state != VstsBuildState.Unknown)
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
