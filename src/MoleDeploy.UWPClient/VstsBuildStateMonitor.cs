using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR.Client;
using MoleDeploy.Contracts;
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

        public VstsBuildStateMonitor(string azureSignalRConnectionString, string hubName)
        {
            var signalR = new AzureSignalR(azureSignalRConnectionString);
            var hubUrl = signalR.GetClientHubUrl(hubName);
            var token = signalR.GenerateAccessToken(hubName);

            _Connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options => {
                options.AccessTokenProvider = () =>
                 {
                     return Task.FromResult(signalR.GenerateAccessToken(hubName));
                 };
            }).Build();

            _Connection.On<VstsBuildStateChangeNotification>("StatusChanged", (noticifation) =>
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
