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
    [Activity(Label = "shopActivity")]
    public class shopActivity : Activity, BottomNavigationView.IOnNavigationItemSelectedListener
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.Shop);
            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation4);
     
            navigation.SetOnNavigationItemSelectedListener(this);
        }
    }
}