using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WhackAMole.MoleDeploy
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private EventProcessorHost _EventProcessorHost;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await Receive();
            });

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await _EventProcessorHost.UnregisterEventProcessorAsync();
            });
        }

        public async Task Receive()
        {
            const string EhConnectionString = "Endpoint=sb://whack-a-mole-eh.servicebus.windows.net/;SharedAccessKeyName=Client;SharedAccessKey=wjN4GsPgS58gw8mgi80+N9+pmD1nHkTtz89NJykb5/A=";
            const string EhEntityPath = "moledeploy";
            const string StorageContainerName = "moledeploy";
            const string StorageAccountName = "dmtcsbwhackamole";
            const string StorageAccountKey = "WfBakVAHLYYBaMZ5znZAe5k49+tZuQaF1FNijh9Nfmv8RigCik17lNGuQ2OAODZftl5VYs5v1HOSB87jsDP6YQ==";
            string StorageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", StorageAccountName, StorageAccountKey);

            _EventProcessorHost = new EventProcessorHost(
                EhEntityPath,
                Microsoft.Azure.EventHubs.PartitionReceiver.DefaultConsumerGroupName,
                EhConnectionString,
                StorageConnectionString,
                StorageContainerName
            );

            var factory = new EventProcessorFactory(ProcessMessagesAsync);

            await _EventProcessorHost.RegisterEventProcessorFactoryAsync(factory);
        }



        private async Task ProcessMessagesAsync(string data)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                textbox_status.Text = data;
            });
        }

        //private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        //{
        //    Console.WriteLine($"Message handler encountered an exception {exceptionReceivedEventArgs.Exception}.");
        //    var context = exceptionReceivedEventArgs.ExceptionReceivedContext;
        //    Console.WriteLine("Exception context for troubleshooting:");
        //    Console.WriteLine($"- Endpoint: {context.Endpoint}");
        //    Console.WriteLine($"- Entity Path: {context.EntityPath}");
        //    Console.WriteLine($"- Executing Action: {context.Action}");

        //    return Task.CompletedTask;
        //}

    }
}
