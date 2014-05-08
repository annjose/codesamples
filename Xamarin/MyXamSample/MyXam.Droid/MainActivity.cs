using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace MyXam.Droid
{
    [Activity(Label = "MyXam", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        Button buttonPrev;
        Button buttonNext;
        TextView textTitle;
        ImageView imageView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            buttonPrev = FindViewById<Button>(Resource.Id.buttonPrev);
            buttonNext = FindViewById<Button>(Resource.Id.buttonNext);
            textTitle = FindViewById<TextView>(Resource.Id.textTitle);
            imageView = FindViewById<ImageView>(Resource.Id.imgPeople);
            imageView.SetImageResource(Resource.Drawable.LizaRed);

            buttonPrev.Click += delegate 
            {
                imageView.SetImageResource(Resource.Drawable.img3);
                textTitle.Text = "Prev button clicked"; 
            };
            buttonNext.Click += delegate 
            {
                imageView.SetImageResource(Resource.Drawable.img2);
                textTitle.Text = "Next button clicked"; 
            };
        }
    }
}

