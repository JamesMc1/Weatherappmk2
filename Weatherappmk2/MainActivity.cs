using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android;
using System.Net;
using Android.Graphics;

namespace Weatherappmk2
{
    [Activity(Label = "Weatherappmk2", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {

        public string City;
        private ImageView myImageView;
        private string StrMetService;
        public string URL { get; set; }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);
            //tie in the ImageView
            myImageView = FindViewById<ImageView>(Resource.Id.Image);

            var btnGetWeather = FindViewById<Button>(Resource.Id.GetWeatherButton);

            btnGetWeather.Click += btnGetWeather_Click;

            SpinnerSetup();


        }

        private void btnGetWeather_Click(object sender, EventArgs e)
        {
            var btnGetWeather = FindViewById<Button>(Resource.Id.GetWeatherButton);

            btnGetWeather.Click += btnGetWeather_Click;

            URL = "http://m.metservice.com/towns/" + City;
            ConnectToNetAndDLTemp();

            btnGetWeather.Text = "Christchurch";
            btnGetWeather.Text = City.ToUpper();

        }

        private void ConnectToNetAndDLTemp()
        {
            var webaddress = new Uri(URL);
            var webclient = new WebClient();
            webclient.DownloadStringAsync(webaddress);
            webclient.DownloadStringCompleted += webclient_DownloadStringCompleted;
        }
        private void webclient_DownloadStringCompleted(Object Sender, DownloadStringCompletedEventArgs e)
        {

            StrMetService = e.Result;
            StrMetService = StrMetService.Replace("\"", string.Empty);

            StrMetService = StrMetService.Remove(0, StrMetService.IndexOf("<body>"));

            var intTempLeft = StrMetService.IndexOf("summary top><div class=ul><h2>") + 30;

            var intTempRight = StrMetService.IndexOf("<span class=temp>") - intTempLeft;

            FindViewById<TextView>(Resource.Id.AllText).Text = StrMetService;

            var Temp = StrMetService.Substring(intTempLeft, intTempRight);
            FindViewById<TextView>(Resource.Id.TempText).Text = Temp + " " + "c";

            var imageBitmap = GetImageBitmapFromUrl(ExtractImagePath());
            myImageView.SetImageBitmap(imageBitmap);

        }

        private string ExtractImagePath()
        {
            StrMetService = StrMetService.Replace("\"", string.Empty);
            var intImageLeft = StrMetService.IndexOf("images-new/wx-icons/") + 20;
            var intImageCount = StrMetService.IndexOf("width=32 height=32") - intImageLeft;
            var strImage = StrMetService.Substring(intImageLeft, intImageCount);
            return "http://m.metservice.com/sites/all/themes/mobile/images-new/wx-icons/" + strImage;
        }

        private Bitmap GetImageBitmapFromUrl(string url)
        {

            Bitmap imageBitmap = null;
            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            var bitmapScaled = Bitmap.CreateScaledBitmap(imageBitmap, 200, 200, true);
            imageBitmap.Recycle();
            return bitmapScaled;
        }
        private void SpinnerSetup()
        {

            var arrayadapter = ArrayAdapter.CreateFromResource(this, Resource.Array.place_array, Android.Resource.Layout.SimpleSpinnerItem);

            arrayadapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
        }
        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            var spinner = (Spinner)sender;
            City = spinner.GetItemAtPosition(e.Position).ToString();
            City = City.ToLower();
            var toast = string.Format("The city is {0}", spinner.GetItemAtPosition(e.Position));
            Toast.MakeText(this, toast, ToastLength.Long).Show();
        }
    }
}     
    

