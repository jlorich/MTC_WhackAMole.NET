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
using System.Drawing;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MoleDeploy.UWPClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private CancellationTokenSource animationCancellationTokenSource;
        private VstsBuildStateMonitor buildStateMonitor;

        private Color STATE_COMPLETE_COLOR = Color.FromArgb(0xFF, 0x47, 0x8e, 0x53);
        private Color STATE_ACTIVE_COLOR = Color.FromArgb(0xFF, 0x1D, 0xAD, 0xD8);
        private Color STATE_INACTIVE_COLOR = Color.FromArgb(0xFF, 0xA0, 0xA0, 0xA0);
        private double STATE_CLIP_DARKNESS_RATIO = .66;

        private Windows.UI.Color SelectedColor = Windows.UI.Color.FromArgb(0xFF, 0xEF, 0x76, 0x7A);
        private int SelectedReplicaCount = 8;

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
            buildStateMonitor = new VstsBuildStateMonitor(Settings.SignalREndpoint, Settings.SignalRAccessKey);
            buildStateMonitor.OnStateBegin += OnStateBegin;
            buildStateMonitor.OnStateEnd += OnStateEnd;

            // Connect SinglaR Hub
            Task.Run(async () => await buildStateMonitor.ConnectToHubAsync());
        }

        private Shape GetElementForState(VstsBuildState state)
        {
            switch (state)
            {
                case VstsBuildState.BuildingApplication:
                    return ellipse_Step1;
                case VstsBuildState.PublishingContainer:
                    return ellipse_Step2;
                case VstsBuildState.UpgradingCluster:
                    return ellipse_Step3;
                case VstsBuildState.DeployComplete:
                    return ellipse_Step4;
                default:
                    return null;
            }
        }

        private Shape GetClipElementForState(VstsBuildState state)
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

            var element = GetClipElementForState(state);

            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                if (state == VstsBuildState.DeployComplete)
                {
                    SetAllColors(STATE_COMPLETE_COLOR);
                    return;
                }

                SetColor(state, STATE_ACTIVE_COLOR);
                await Rotate(element, animationCancellationTokenSource.Token);
            });

        }

        private async void OnStateEnd(object sender, VstsBuildState state)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                SetColor(state, STATE_COMPLETE_COLOR);
            });
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

        private void SetAllColors(Color color)
        {
            foreach (var state in Enum.GetValues(typeof(VstsBuildState)))
            {
                SetColor((VstsBuildState)state, color);
            }
        }

        private void SetColor(VstsBuildState state, Color color)
        {
            var e = GetElementForState(state);
            var c = GetClipElementForState(state);

            if (e == null || c == null) return;

            var darkColor = DarkenColor(color, STATE_CLIP_DARKNESS_RATIO);

            var ewColor = Windows.UI.Color.FromArgb(color.A, color.R, color.G, color.B);
            var cwColor = Windows.UI.Color.FromArgb(darkColor.A, darkColor.R, darkColor.G, darkColor.B);

            e.Fill = new SolidColorBrush(ewColor);
            c.Fill = new SolidColorBrush(cwColor);
        }

        private static Color DarkenColor(Color color, double darkenAmount)
        {
            HSLColor hslColor = new HSLColor(color);
            hslColor.Luminosity *= darkenAmount; // 0 to 1
            return hslColor;
        }


        private void button_deploy_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async () =>
            {
                await SubmitBuild();
            });
        }

        private async Task SubmitBuild()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                SetAllColors(STATE_INACTIVE_COLOR);
                SetColor(VstsBuildState.BuildingApplication, STATE_ACTIVE_COLOR);
            });

            var url = "https://mtcden-sandbox-demo-whack-a-mole-vsts-func.azurewebsites.net/api/SubmitBuild";
            var colorString = Color.FromArgb(SelectedColor.A, SelectedColor.R, SelectedColor.G, SelectedColor.B).ToArgb().ToString("X8").Substring(2, 6);

            var request = new SubmitBuildRequest()
            {
                Color = colorString,
                ReplicaCount = SelectedReplicaCount
            };

            var body = JsonConvert.SerializeObject(request);
            var client = new HttpClient();
            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var result = await client.PostAsync(url, content);
            var resultBody = result.Content.ReadAsStringAsync();
        }

        private void ColorChanged(object sender, RoutedEventArgs e)
        {
            var checkboxes = new List<CheckBox>()
            {
                checkbox_Brown,
                checkbox_Blue,
                checkbox_Green,
                checkbox_Orange,
                checkbox_Pink,
                checkbox_Red,
                checkbox_Teal,
                checkbox_Yellow
            };

            var selected = (CheckBox)sender;

            foreach(var checkbox in checkboxes.Where(a => a != selected && a != null))
            {
                checkbox.IsChecked = false;
            }

            SelectedColor = ((SolidColorBrush)selected.Background).Color;
        }

        private void Slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            SelectedReplicaCount = (int)((Slider)sender).Value;
        }
    }
}
