using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViktorApp
{
    [Activity(Label = "Result")]
    public class Result : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            StartMedia();


            SetContentView(Resource.Layout.result);
            //Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            Button btnRestart = FindViewById<Button>(Resource.Id.btnRestart);
            Button btnShare = FindViewById<Button>(Resource.Id.btnShare);
            Button btnQuit = FindViewById<Button>(Resource.Id.btnQuit);

            btnRestart.Click += delegate
            {
                try
                {
                    Intent intent = new Intent(this, typeof(MainActivity));
                    StartActivity(intent);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                }
            };



            btnQuit.Click += delegate
            {
                System.Diagnostics.Process.GetCurrentProcess().Kill();
            };
        }
        MediaPlayer player;

        public bool StartMedia()
        {
            player = MediaPlayer.Create(this, Resource.Raw.win);
            player.Start();
            return true;
        }
    }
}