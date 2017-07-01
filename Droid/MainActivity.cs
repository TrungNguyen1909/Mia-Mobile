﻿using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Speech;
using Plugin.TextToSpeech;
using Plugin.SpeechToText;
using DeviceOrientation.Forms.Plugin.Droid;

namespace Mia.Droid
{
    [Activity(Label = "Mia.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation,WindowSoftInputMode = SoftInput.AdjustPan)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            CrossTextToSpeech.Current.Init();
            DeviceOrientationImplementation.Init();
            LoadApplication(new App());
            XFGloss.Droid.Library.Init(this,bundle);
        }
		public override void OnConfigurationChanged(global::Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged(newConfig);
			DeviceOrientationImplementation.NotifyOrientationChange(newConfig);
		}
    }
}