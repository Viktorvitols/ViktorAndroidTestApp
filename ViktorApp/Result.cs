using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Test;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace ViktorApp
{
    [Activity(Label = "Result")]
    public class Result : Activity
    {


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            string totalScore = this.Intent.GetStringExtra("totalScore");
            //SetContentView(Resource.Layout.quizz);
            StartMedia();
            SetContentView(Resource.Layout.result);


            TextView txtResult = FindViewById<TextView>(Resource.Id.txtResult);
            txtResult.Text = totalScore;
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

            btnShare.Click += (s, e) =>
            {
                Share.RequestAsync(new ShareTextRequest
                {
                    Text = "Check out my score: " + $"{ txtResult.Text}", Title = "Math quizz"
                });
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