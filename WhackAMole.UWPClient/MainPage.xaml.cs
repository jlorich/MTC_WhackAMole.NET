using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WhackAMole.UWPClient.Controls;
using WhackAMole.UWPClient.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WhackAMole.UWPClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private WhackSpace _whackSpace;
        private Dictionary<string, string> _moleMap = new Dictionary<string, string>();
        private const string SETTINGS_FILE_LOCATION = "appsettings.json";

        private WhackSettings _Settings;

        private WhackSettings Settings
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
                _Settings = JsonConvert.DeserializeObject<WhackSettings>(data);

                return _Settings;
            }
        }
        
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += async (s, e) => {
                MolePen.Clip = new RectangleGeometry() { Rect = new Rect(0, 0, MolePen.ActualWidth, MolePen.ActualHeight) };
                if (_whackSpace == null)
                {
                    _whackSpace = new WhackSpace(MolePen.ActualWidth, MolePen.ActualHeight, Settings);
                    DataContext = _whackSpace;
                    _whackSpace.Inactive += async (s1, e1) =>
                    {
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
                            () =>
                            {
                                this.Frame.GoBack();
                            });
                    };
                }

                Debug.WriteLine($"start size: {this.ActualWidth} - {this.ActualHeight}");
                Debug.WriteLine($"mole start size: {MolePen.ActualWidth} - {MolePen.ActualHeight}");
                await Start();
            };
            
            this.SizeChanged += (s, e) => {
                if (_whackSpace == null)
                    return;
                Debug.WriteLine($"old size: {e.PreviousSize} new size: {e.NewSize}");
                Debug.WriteLine($"mole size: {MolePen.ActualWidth} - {MolePen.ActualHeight}");
                _whackSpace.Width = MolePen.ActualWidth;
                _whackSpace.Height = MolePen.ActualHeight;
            };
        }



        private async Task Start()
        {
            Debug.WriteLine("starting");
            await _whackSpace.SetupAsync(MolePen);
            _whackSpace.Start();
        }
    }
}
