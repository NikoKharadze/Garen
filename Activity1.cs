using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Google.Android.Material.BottomNavigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App1
{
    [Activity(Label = "offersActivity")]
    
    public class offersActivity : Activity ,BottomNavigationView.IOnNavigationItemSelectedListener
    {
        Button button_quiz;
        Button button_wheel;
        Button button_prizes;
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
          
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.offersActivity);

            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation1);
            navigation.SetOnNavigationItemSelectedListener(this);
            button_quiz = FindViewById<Button>(Resource.Id.btn_quiz);
            button_wheel = FindViewById<Button>(Resource.Id.btn_wheel);
            button_prizes = FindViewById<Button>(Resource.Id.btn_prizes);
            button_wheel.Click += onclk;
            button_prizes.Click += onclkprz;
            button_quiz.Click += onclkquiz;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        private void onclk(object sender, EventArgs eventArgs) {

            StartActivity(typeof(WheelActivity));
        }

        private void onclkprz(object sender, EventArgs eventArgs)
        {

            StartActivity(typeof(Prizes));
        }

        private void onclkquiz(object sender, EventArgs eventArgs)
        {

            Intent intent = new Intent(this, typeof(QuizActivity));
            StartActivityForResult(intent, 1); // Use a unique request code (here, 1)
        }




        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == 1) // Use the same request code you used when starting QuizActivity
            {
                if (resultCode == Result.Ok)
                {
                    // Quiz completed successfully, and the user answered all questions correctly
                    Toast.MakeText(this, "Congratulations! Quiz completed successfully.", ToastLength.Short).Show();
                }
                else if (resultCode == Result.Canceled)
                {
                    // Quiz completed, but the user got at least one answer incorrect
                    Toast.MakeText(this, "Quiz completed with incorrect answers.", ToastLength.Short).Show();
                }
            }
        }

    }
}