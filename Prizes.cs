using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomNavigation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using static App1.QuizActivity;

namespace App1
{
    [Activity(Label = "Prizes")]
    public class Prizes : Activity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_home:
                    StartActivity(typeof(MainActivity));
                    return true;
                case Resource.Id.navigation_offer:
                    // Open a new activity for Offers
                    StartActivity(typeof(offersActivity));
                    return true;
                case Resource.Id.navigation_notifications:
                    StartActivity(typeof(shopActivity));
                    return true;
            }
            return false;
        }
        private const string BaseUrl = "http://10.42.134.5:500";
        private const string Endpoint = "/api/questions";
        TextView prz1;
        private TextView prz2;
        private TextView prz3;
        private RelativeLayout rec1;
        private RelativeLayout rec2;
        private RelativeLayout rec3;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.prize_layout);
            prz1 = FindViewById<TextView>(Resource.Id.prize1);
            prz2 = FindViewById<TextView>(Resource.Id.prize2);
            prz3 = FindViewById<TextView>(Resource.Id.prize3);
            rec1 = FindViewById<RelativeLayout>(Resource.Id.rectangle1);
            rec2 = FindViewById<RelativeLayout>(Resource.Id.rectangle2);
            rec3 = FindViewById<RelativeLayout>(Resource.Id.rectangle3);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation5);
            navigation.SetOnNavigationItemSelectedListener(this);
            rec1.Click += OnRectangleClick;
            rec2.Click += OnRectangleClick;
            rec3.Click += OnRectangleClick;
            GetPrizesFromServer();
        }




        private void OnRectangleClick(object sender, EventArgs e)
        {
            // Determine which rectangle was clicked
            RelativeLayout clickedRectangle = sender as RelativeLayout;

            // Show confirmation dialog
            ShowConfirmationDialog(clickedRectangle);
        }

        // Method to show a confirmation dialog
        private void ShowConfirmationDialog(RelativeLayout clickedRectangle)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetMessage("Do you want cash this prize?")
                   .SetPositiveButton("Yes", (sender, args) =>
                   {
                       clickedRectangle.Visibility = ViewStates.Gone;
                       
                   })
                   .SetNegativeButton("No", (sender, args) =>
                   {
                       // User clicked "No", do nothing or handle accordingly
                   })
                   .Create()
                   .Show();
        }

        private async void GetPrizesFromServer()
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(BaseUrl);

                    HttpResponseMessage response = await httpClient.GetAsync(Endpoint);

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonContent = await response.Content.ReadAsStringAsync();

                        PrizeList prizeList = JsonConvert.DeserializeObject<PrizeList>(jsonContent);

                        // Ensure there are enough prizes
                        if (prizeList.Prizes.Count >= 3)
                        {
                            // Get the first three prizes
                            List<Prize> firstThreePrizes = prizeList.Prizes.Take(3).ToList();

                            // Update TextViews with prize information
                            prz1.Text = $"Expiration Date: {firstThreePrizes[0].ExpirationDate}, Megabytes Won: {firstThreePrizes[0].MegabytesWon}";
                            prz2.Text = $"Expiration Date: {firstThreePrizes[1].ExpirationDate}, Megabytes Won: {firstThreePrizes[1].MegabytesWon}";
                            prz3.Text = $"Expiration Date: {firstThreePrizes[2].ExpirationDate}, Megabytes Won: {firstThreePrizes[2].MegabytesWon}";

                            // You can use 'firstThreePrizes' for further processing or display.
                        }
                        else
                        {
                            Console.WriteLine("There are not enough prizes available.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }

    }

    public class Prize
    {
        [JsonProperty("expirationDate")]
        public DateTime ExpirationDate { get; set; }

        [JsonProperty("megabytesWon")]
        public int MegabytesWon { get; set; }
    }

    public class PrizeList
    {
        [JsonProperty("prizes")]
        public List<Prize> Prizes { get; set; }
    }
    
}