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
        int yourScore;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            string totalScore = this.Intent.GetStringExtra("totalScore");
            yourScore = this.Intent.GetIntExtra("yourScore", 0);
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
            if (yourScore > 6)
            {
                player = MediaPlayer.Create(this, Resource.Raw.win);
            } else
            {
                player = MediaPlayer.Create(this, Resource.Raw.fail);
            }
            player.Start();
            return true;
        }
    }
}