using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Streams;
using Windows.Networking.Connectivity;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Globalization;
using System.Threading;

using Windows.Devices.Geolocation;


using Windows.Services.Maps;
using Windows.Storage;
using Windows.UI.Xaml.Controls.Maps;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SteveIOT1
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private DispatcherTimer timer;
        public MainPage()
        {
            this.InitializeComponent();

            timer = new DispatcherTimer();


            MainBlock.Text = "Weather";
            SubBlock.Text = "Starting up...";
            IPAddress currentip = GetIpAddress();
            ipinformation.Text = currentip.ToString();
            btnRefresh.Content = "Refresh";

            GetWX();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMinutes(15);
            timer.Tick += Timer_Tick;
            timer.Start();




        }

        private void Timer_Tick(object sender, object e)
        {
            GetWX();
        }

        private async void GetWX()
        {
            var url = "http://api.wunderground.com/api/cadcec0fb1ba94f1/conditions/q/pws:KVAMECHA26.json";

            try
            {
                Uri geturi = new Uri(url);
                System.Net.Http.HttpClient client = new System.Net.Http.HttpClient();
                System.Net.Http.HttpResponseMessage responseGet = await client.GetAsync(geturi);
                string strresponse = await responseGet.Content.ReadAsStringAsync();

                RootObject response = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(strresponse);

                MainBlock.Text = response.current_observation.observation_location.full;

                SubBlock.Text = response.current_observation.temp_f.ToString() + "° F";
                SubBlock2.Text = response.current_observation.relative_humidity + " Humidity";

                //string imageURL = "http://icons.wxug.com/i/c/j/" & response.current_observation.icon_url    clear.gif";
                string imageURL = response.current_observation.icon_url;

                wximage.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(imageURL, UriKind.Absolute));

                var lat = response.current_observation.display_location.latitude;
                var longitude = response.current_observation.display_location.longitude ;


           //     BasicGeoposition mapCenter = new BasicGeoposition();
             //   mapCenter.Latitude = double.Parse(lat);
               // mapCenter.Longitude = double.Parse(longitude);

                // Specify a known location.
                BasicGeoposition snPosition = new BasicGeoposition();
                snPosition.Latitude = double.Parse(lat);
                snPosition.Longitude = double.Parse(longitude);

                Geopoint snPoint = new Geopoint(snPosition);

                // Create a MapIcon.
                MapIcon mapIcon1 = new MapIcon();
                mapIcon1.Location = snPoint;
                mapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
                mapIcon1.Title = SubBlock.Text;
                mapIcon1.ZIndex = 0;

                mapIcon1.Image =  RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/icon_comms.png"));

                // Add the MapIcon to the map.
                MapControl1.MapElements.Add(mapIcon1);

                // Center the map over the POI.
                MapControl1.Center = snPoint;
                MapControl1.ZoomLevel = 14;
            


                //Geopoint hwPoint = new Geopoint(mapCenter);

            //MapControl.Style = Windows.UI.Xaml.Controls.Maps.MapStyle.AerialWithRoads;

            //MapControl.Center = new Geopoint(mapCenter);

            //Geopoint snPoint = new Geopoint(mapCenter);
            ////MapScene hwScene = MapScene.CreateFromLocationAndRadius(hwPoint,
            ////                                                         80, /* show this many meters around */
            ////                                                         0, /* looking at it to the North*/
            ////                                                         60 /* degrees pitch */);
            ////// Set the 3D view with animation.
            //// await MapControl.TrySetSceneAsync(hwScene, MapAnimationKind.Bow);


            //MapControl.ZoomLevel = 10;
            //MapControl.LandmarksVisible = true;

            //// Create a MapIcon.
            //MapIcon mapIcon1 = new MapIcon();
            //mapIcon1.Location = snPoint;
            //mapIcon1.NormalizedAnchorPoint = new Point(0.5, 1.0);
            //mapIcon1.Title = "KVAMECHA26";
            //mapIcon1.ZIndex = 0;

            //// Add the MapIcon to the map.
            //MapControl.MapElements.Add(mapIcon1);




            //MapControl.MapElements.Clear();



        }

            catch (Exception ex)
            {
                MainBlock.Text = ex.Message;
            }




        }


        //private async Task<string> ReverseGeocode(LocationDto location)
        //{
        //    // Location to reverse geocode.
        //    BasicGeoposition basicLocation = new BasicGeoposition();
        //    basicLocation.Latitude = location.Latitude;
        //    basicLocation.Longitude = location.Longitude;
        //    Geopoint pointToReverseGeocode = new Geopoint(basicLocation);

        //    // Reverse geocode the specified geographic location.
        //    MapLocationFinderResult result =
        //        await MapLocationFinder.FindLocationsAtAsync(pointToReverseGeocode);

        //    // If the query returns results, display the name of the town
        //    // contained in the address of the first result.
        //    if (result.Status == MapLocationFinderStatus.Success)
        //    {
        //        return result.Locations[0].Address.Town +
        //            ", " + result.Locations[0].Address.Country;
        //    }

        //    return string.Empty;
        //}




        public IPAddress GetIpAddress()
        {
            var hosts = NetworkInformation.GetHostNames();
            foreach (var host in hosts)
            {
                IPAddress addr;
                if (!IPAddress.TryParse(host.DisplayName, out addr)) continue;
                if (addr.AddressFamily != AddressFamily.InterNetwork) continue;
                return addr;
            }
            return null;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            GetWX();
        }
    }
}
