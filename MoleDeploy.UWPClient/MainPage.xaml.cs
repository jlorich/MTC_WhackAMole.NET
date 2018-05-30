using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.AspNetCore.SignalR.Client;
using Windows.Storage;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MoleDeploy.UWPClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private CancellationTokenSource animationCancellationTokenSource;

        public MainPage()
        {
            this.InitializeComponent();
            InitilizeHub();
        }

        private async void InitilizeHub()
        {
            var endpoint = "https://mtcden-sandbox-demo-whack-a-mole.service.signalr.net";// ApplicationData.Current.LocalSettings.Values["moleServiceEndpoint"] as string;
            var accessKey = "HBCBkIRl/CqVBMbG9VUzrQ4Cp9msXAVBKVPeCzpEkR0=";// ApplicationData.Current.LocalSettings.Values["moleServiceEndpoint"] as string;

            var sr = new AzureSignalR($"Endpoint={endpoint};AccessKey={accessKey}");
            var hubUrl = sr.GetClientHubUrl("Status");
            var token = sr.GenerateAccessToken("Status");

            var connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options => {
                options.Headers = new Dictionary<string, string> { { "Authorization", string.Format("bearer {0}", token) } };
            })
            .Build();

            CancellationTokenSource lastTokenSource = null;

            connection.On<string>("StatusChanged", async (message) =>
            {
                if (lastTokenSource != null)
                {
                    lastTokenSource.Cancel();
                }

                lastTokenSource = new CancellationTokenSource();

                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    await Rotate(ellipse_Step1Clip, lastTokenSource.Token);
                });
            });

            try
            {
                await connection.StartAsync();

            }
            catch (Exception ex)
            {

            }
        }

        private async Task Rotate(UIElement element, CancellationToken cancellationToken) {
            Storyboard storyboard1 = new Storyboard();

            cancellationToken.Register(async () =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    storyboard1.Stop();
                });
            });

            element.RenderTransform = new RotateTransform();

            DoubleAnimation rotateAnimation = new DoubleAnimation();

            rotateAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 2, 500));
            rotateAnimation.From = 240;
            rotateAnimation.To = 600;
            rotateAnimation.RepeatBehavior = RepeatBehavior.Forever;
            rotateAnimation.EasingFunction = new SineEase
            {
                EasingMode = EasingMode.EaseInOut
            };

            Storyboard.SetTarget(rotateAnimation, element.RenderTransform);
            Storyboard.SetTargetProperty(rotateAnimation, "Angle");

            storyboard1.Children.Add(rotateAnimation);

            await storyboard1.BeginAsync();
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
