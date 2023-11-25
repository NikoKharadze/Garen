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
using static SkiaSharp.Views.Android.SKCanvasView;
using SkiaSharp.Views;
using SkiaSharp.Views.Android;
using SkiaSharp.Views.Forms;
using System.Drawing;
using SKCanvasView = SkiaSharp.Views.Android.SKCanvasView;
using SkiaSharp;
using static Android.Provider.MediaStore.Audio;
using Android.Views.Animations;
using Android.Graphics;
using static Android.Icu.Text.IDNA;
using Google.Android.Material.Snackbar;
using Android.Icu.Util;

namespace App1
{
    [Activity(Label = "WheelActivity")]
    public class WheelActivity : Activity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        TextView counter;
        TextView bet;
        Button minus;
        Button plus;
        Button spin;
        SKCanvasView skiaSurfaceView;
        SKCanvasView trg;
        bool isSpinning = false;
        bool created = false;

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
            SetContentView(Resource.Layout.Wheel);


            BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.navigation2);
            navigation.SetOnNavigationItemSelectedListener(this);
            trg = FindViewById<SkiaSharp.Views.Android.SKCanvasView>(Resource.Id.skiaSurfaceView1);
            trg.PaintSurface += OnSkiaSurfaceViewPaintTrianlge;
            skiaSurfaceView = FindViewById<SkiaSharp.Views.Android.SKCanvasView>(Resource.Id.skiaSurfaceView);
            skiaSurfaceView.PaintSurface += OnSkiaSurfaceViewPaintSurface;
            bet = FindViewById<TextView>(Resource.Id.text_bet);
            minus = FindViewById<Button>(Resource.Id.btn_betminus);
            plus = FindViewById<Button>(Resource.Id.btn_betplus);
            counter = FindViewById<TextView>(Resource.Id.coin_count);
            spin = FindViewById<Button>(Resource.Id.btn_spin);
            minus.Click += min;
            plus.Click += pl;
            spin.Click += SpinButtonClick;
        }

        private void OnSkiaSurfaceViewPaintTrianlge(object sender, SkiaSharp.Views.Android.SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;
            using (var trianglePath = new SKPath())
            {
                float triangleHeight = -150;
                float triangleBase = 100;
                float triangleTipX = info.Width / 2;
                float triangleTipY = info.Height / 2 - 480 - triangleHeight / 2;

                trianglePath.MoveTo(triangleTipX, triangleTipY);
                trianglePath.LineTo(triangleTipX - triangleBase / 2, triangleTipY + triangleHeight);
                trianglePath.LineTo(triangleTipX + triangleBase / 2, triangleTipY + triangleHeight);
                trianglePath.Close();

                SKPaint trianglePaint = new SKPaint
                {
                    Style = SKPaintStyle.Fill,
                    Color = SKColors.Red,
                };

                canvas.DrawPath(trianglePath, trianglePaint);
            }
        }

        private void OnSkiaSurfaceViewPaintSurface(object sender, SkiaSharp.Views.Android.SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            if (!created)
            {
                canvas.Clear();

                SKPaint paint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.Wheat,
                    StrokeWidth = 25
                };

                // Draw the circle outline
                canvas.DrawCircle(info.Width / 2, info.Height / 2, 450, paint);

                // Define variables for filling the circle
                int numDivisions = 30;
                float angleIncrement = 360f / numDivisions;

                // Fill the circle with alternating black and blue halves
                for (int i = 0; i < numDivisions; i++)
                {
                    SKPaint fillPaint = new SKPaint
                    {
                        Style = SKPaintStyle.Fill,
                        Color = (i % 2 == 0) ? SKColors.Black : SKColors.White,
                    };

                    float startAngle = i * angleIncrement;
                    float endAngle = (i + 1) * angleIncrement;

                    using (var path = new SKPath())
                    {
                        float radius = 450;
                        float centerX = info.Width / 2;
                        float centerY = info.Height / 2;

                        // Use a square bounding box
                        SKRect boundingBox = new SKRect(centerX - radius, centerY - radius, centerX + radius, centerY + radius);

                        path.MoveTo(centerX, centerY);
                        path.ArcTo(boundingBox, startAngle, endAngle - startAngle, false);
                        path.Close();

                        canvas.DrawPath(path, fillPaint);


                    }

                }
                created = true;
            }

            if (isSpinning)
            {
                // Rotate the canvas based on the current time to create a spinning effect
                float rotationDegrees = (DateTime.Now.Millisecond % 360) * 0.5f;
                canvas.RotateDegrees(1, info.Width / 2, info.Height / 2);


                //isSpinning = false;
            }

        }
        private void min(object sender, EventArgs arg)
        {
            int bt = Convert.ToInt32(bet.Text);
            bt = bt == 1 ? 1 : bt - 1;
            bet.Text = bt.ToString();
        }
        private void pl(object sender, EventArgs arg)
        {
            int bt = Convert.ToInt32(bet.Text);
            int maximum = Convert.ToInt32(counter.Text.Split(" ")[2]);
            bt = bt == 3 ? 3 : bt + 1;
            bt = bt > maximum ? maximum : bt;
            bet.Text = bt.ToString();
        }

        private void SpinButtonClick(object sender, EventArgs e)
        {
            int left = int.Parse(counter.Text.Split(' ')[2]);
            View view = (View)sender;
            if (left >= int.Parse(bet.Text))
            {
                if (!isSpinning)
                {
                    left -= int.Parse(bet.Text);
                    counter.Text = "coins left " + left;

                    isSpinning = true;

                    // Use an animation to gradually rotate the wheel
                    RotateAnimation rotateAnimation = new RotateAnimation(0, 360, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
                    rotateAnimation.Duration = 5000; // Adjust the duration as needed
                    rotateAnimation.RepeatCount = 0;
                    rotateAnimation.AnimationEnd += (s, a) => OnSpinAnimationEnd(view);

                    // Start the animation
                    skiaSurfaceView.StartAnimation(rotateAnimation);

                    // You may also want to disable the spin button while the wheel is spinning
                    spin.Enabled = false;



                }
                else
                {
                    isSpinning = false;

                    // Stop the animation
                    skiaSurfaceView.ClearAnimation();

                    // Re-enable the spin button
                    spin.Enabled = true;
                }
            }
            else {
                Snackbar.Make(view, "not enough points to spin", Snackbar.LengthLong).SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
            }

        }

        private void OnSpinAnimationEnd(View view)
        {
            isSpinning = false;
            skiaSurfaceView.ClearAnimation();
            // Re-enable the spin button
            spin.Enabled = true;

            bool result;
            int difficulty;
            if (int.Parse(bet.Text) == 1)
            {
                difficulty = 1;
                result = DeterminePrize(0.5);
            }
            else if (int.Parse(bet.Text) == 2)
            {
                difficulty = 2;
                result = DeterminePrize(0.66);
            }
            else
            {
                difficulty = 3;
                result = DeterminePrize(0.83);
            }
            int winning = 0;
            
            String output = "Lost! :/";
            if (result)
            {
                winning = PrizeAmount(difficulty);
                output = "Winner! prize: ";
                output += winning.ToString() +"MB";
            }


            Snackbar.Make(view, output, Snackbar.LengthLong).SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();

        }


        private bool DeterminePrize(double win_probabilty) {

            Random rnd= new Random();
            double given= rnd.NextDouble();
            if (given < win_probabilty) {
                return false;
            }
            return true;
           
        }

        private int PrizeAmount(int difficulty) {
            Random rnd = new Random();
            int win = rnd.Next(100,501);
            win = win / difficulty;
            if (win < 50) {
                win = 50;
            }
            return win;
        }

    }
}