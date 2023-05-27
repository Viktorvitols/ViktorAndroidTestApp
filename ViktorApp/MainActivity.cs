using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Webkit;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Runtime.InteropServices.ComTypes;


[assembly: UsesPermission(Android.Manifest.Permission.WriteExternalStorage)]
[assembly: UsesPermission(Android.Manifest.Permission.Internet)]

namespace ViktorApp
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        readonly string[] Permission = { Android.Manifest.Permission.WriteExternalStorage };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            RequestPermissions(Permission, 0);
            Tiesibas();

            Button buttonStart = FindViewById<Button>(Resource.Id.btnStart);
            Spinner spinner = FindViewById<Spinner>(Resource.Id.category);
            List<string> categories = new List<string>();
            categories.Add("Addition");
            categories.Add("Subtraction");

            var adapter = new ArrayAdapter<string>
                           (this, Android.Resource.Layout.SimpleSpinnerDropDownItem, categories);
            spinner.Adapter = adapter;

            string catName = spinner.SelectedItem.ToString();
            //int catIndex = spinner.SelectedItemPosition;

            buttonStart.Click += (sender, args) =>
            {
                if (spinner.SelectedItem != null)
                {
                    try
                    {
                        Intent intent = new Intent(this, typeof(Quizz));
                        //intent.PutExtra("catIndex", catIndex);
                        intent.PutExtra("catName", catName);
                        StartActivity(intent);
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                    }
                }
            };



        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        private void Tiesibas()
        {
            if (CheckSelfPermission(Android.Manifest.Permission.WriteExternalStorage)
                != Android.Content.PM.Permission.Granted)
            {
                RequestPermissions(Permission, 0);
            }
        }


    }
}