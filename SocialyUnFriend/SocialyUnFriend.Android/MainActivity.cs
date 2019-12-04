using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using SocialyUnFriend.Common;
using Android.OS;
using FFImageLoading.Forms.Platform;
using Prism;
using Prism.Ioc;
using System.IO;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Plugin.CurrentActivity;
using CarouselView.FormsPlugin.Android;
using SocialyUnFriend.DependencyServcices;
using SocialyUnFriend.Droid.Services;

namespace SocialyUnFriend.Droid
{
    [Activity(Label = "SocialPost", Icon = "@drawable/icon", Theme = "@style/Theme.Splash", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        public static Activity Instance;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;



            SetTheme(Resource.Style.MainTheme);
             
            base.OnCreate(savedInstanceState);

            Instance = this;

            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            global::Xamarin.Forms.Forms.SetFlags("CollectionView_Experimental");

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            CarouselViewRenderer.Init();

            CachedImageRenderer.Init(true);

            CachedImageRenderer.InitImageViewHandler();

            AppCenter.Start(Constants.AppSecretAndroid, typeof(Analytics), typeof(Crashes));

            LoadApplication(new App(new AndroidInitializer()));
        }

        public class AndroidInitializer : IPlatformInitializer
        {
            public void RegisterTypes(IContainerRegistry containerRegistry)
            {
                //do the platform specific registrations here..

                containerRegistry.Register<ILocationSettings, LocationSettings>();
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}