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
using Windows.Storage;
using Microsoft.Toolkit.Uwp.UI.Animations;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using MoleDeploy.Contracts;
using System.Net.Http;
using MoleDeploy.Vsts;
using Newtonsoft.Json;
using System.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MoleDeploy.UWPClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private CancellationTokenSource animationCancellationTokenSource;
        private VstsBuildStateMonitor buildStateManager;

        private const string SETTINGS_FILE_LOCATION = "appsettings.json";
        private DeployClientSettings _Settings;

        private DeployClientSettings Settings
        {
            get
            {
                if (_Settings != null)
                {
                    return _Settings;
                }

                var packageFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var file = packageFolder.GetFileAsync(SETTINGS_FILE_LOCATION).GetAwaiter().GetResult();
                var data = FileIO.ReadTextAsync(file).GetAwaiter().GetResult();
                _Settings = JsonConvert.DeserializeObject<DeployClientSettings>(data);

                return _Settings;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            buildStateManager = new VstsBuildStateMonitor(Settings.SignalREndpoint, Settings.SignalRAccessKey);
            buildStateManager.OnStateBegin += OnStateBegin;
            buildStateManager.OnStateEnd += OnStateEnd;
            buildStateManager.InitilizeHubAsync().GetAwaiter().GetResult();
        }

        private UIElement GetElementForState(VstsBuildState state)
        {
            switch (state)
            {
                case VstsBuildState.BuildingApplication:
                    return ellipse_Step1Clip;
                case VstsBuildState.PublishingContainer:
                    return ellipse_Step2Clip;
                case VstsBuildState.UpgradingCluster:
                    return ellipse_Step3Clip;
                case VstsBuildState.DeployComplete:
                    return ellipse_Step4Clip;
                default:
                    return null;
            }
        }

        private async void OnStateBegin(object sender, VstsBuildState state)
        {
            if (animationCancellationTokenSource != null)
            {
                animationCancellationTokenSource.Cancel();
            }

            animationCancellationTokenSource = new CancellationTokenSource();

            var element = GetElementForState(state);

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                await Rotate(element, animationCancellationTokenSource.Token);
            });

        }

        private void OnStateEnd(object sender, VstsBuildState state)
        {

        }

        private async Task Rotate(UIElement element, CancellationToken cancellationToken)
        {
            Storyboard storyboard = new Storyboard();

            cancellationToken.Register(async () =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    storyboard.RepeatBehavior = new RepeatBehavior(0);
                });
            });

            element.RenderTransform = new RotateTransform();

            DoubleAnimation rotateAnimation = BuildAnimation();


            Storyboard.SetTarget(rotateAnimation, element.RenderTransform);
            Storyboard.SetTargetProperty(rotateAnimation, "Angle");

            storyboard.Children.Add(rotateAnimation);

            await storyboard.BeginAsync();
        }

        private DoubleAnimation BuildAnimation(bool repeat = true)
        {
            DoubleAnimation rotateAnimation = new DoubleAnimation();

            rotateAnimation.Duration = new Duration(new TimeSpan(0, 0, 0, 2, 500));
            rotateAnimation.From = 240;
            rotateAnimation.To = 600;
            rotateAnimation.RepeatBehavior = RepeatBehavior.Forever;
            rotateAnimation.EasingFunction = new SineEase
            {
                EasingMode = EasingMode.EaseInOut
            };

            return rotateAnimation;
        }

        private void button_deployRed_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await SubmitBuild("D62D1A");
            });
        }

        private void button_deployOrange_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await SubmitBuild("FA8B37");
            });
        }

        private void button_deployPurple_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await SubmitBuild("531868");
            });
        }

        private async Task SubmitBuild(string color)
        {
            var url = "https://mtcden-sandbox-demo-whack-a-mole-vsts-func.azurewebsites.net/api/SubmitBuild";

            var request = new SubmitBuildRequest()
            {
                Color = color
            };

            var body = JsonConvert.SerializeObject(request);
            var client = new HttpClient();
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var result = await client.PostAsync(url, content);
            var resultBody = result.Content.ReadAsStringAsync();
        }
    }
}
