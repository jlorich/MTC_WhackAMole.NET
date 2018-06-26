using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Client;
using MoleDeploy.Contracts;
using Newtonsoft.Json;
using MoleDeploy.SignalR;

namespace MoleDeploy.UWPClient
{

    public class VstsBuildStateMonitor
    {
        public delegate void BuildStateChangeHandler(object sender, VstsBuildState state);

        public event BuildStateChangeHandler OnStateBegin;
        public event BuildStateChangeHandler OnStateEnd;

        private const string endpoint = "https://mtcden-sandbox-demo-whack-a-mole.service.signalr.net";// ApplicationData.Current.LocalSettings.Values["moleServiceEndpoint"] as string;
        private const string accessKey = "HBCBkIRl/CqVBMbG9VUzrQ4Cp9msXAVBKVPeCzpEkR0=";// ApplicationData.Current.LocalSettings.Values["moleServiceEndpoint"] as string;

        private VstsBuildState state;

        public VstsBuildStateMonitor()
        {
            InitilizeHub(endpoint, accessKey);
        }

        private async void InitilizeHub(string endpoint, string accessKey)
        {
            var signalR = new AzureSignalR($"Endpoint={endpoint};AccessKey={accessKey}");
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

            try
            {
                await connection.StartAsync();
            }
            catch (Exception ex)
            {

            }
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
