﻿using DryIoc;
using Prism;
using Prism.Ioc;
using SocialyUnFriend.NetworkController;
using SocialyUnFriend.Services;
using Xamarin.Forms;
using Xamarin.Essentials.Implementation;
using Xamarin.Essentials.Interfaces;
using SocialyUnFriend.LocalDB;
using SocialyUnFriend.Repositories;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using SocialyUnFriend.Common;
using SocialyUnFriend.ViewModels;
using SocialyUnFriend.CustomDialogs;
using XFShimmerLayout.Controls;

namespace SocialyUnFriend
{
    [AutoRegisterForNavigation]
    public partial class App
    {

        public App() : this(null) { }
        public App(IPlatformInitializer initializer) : base(initializer) { }


        protected override async void OnInitialized()
        {
           
            AppCenter.Start(string.Format("ios={0};android={1};uwp={2}",Constants.AppSecretiOS,Constants.AppSecretAndroid,Constants.AppSecretUWP), 
                            typeof(Analytics), typeof(Crashes));

            InitializeComponent();

            var density = Xamarin.Essentials.DeviceDisplay.MainDisplayInfo.Density;
            ShimmerLayout.Init(density);


            if (Current.Properties.ContainsKey(Constants.IsWelcomePageVisible) && (bool)Current.Properties[Constants.IsWelcomePageVisible])
                await NavigationService.NavigateAsync("NavigationPage/LoginPage");
            else
                await NavigationService.NavigateAsync("NavigationPage/WelcomePage");

               
         

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();

            //Regiter for dialogs

            containerRegistry.RegisterDialog<PostDialog, PostDialogViewModel>();

            // Register NetWerk Services,,
           
            containerRegistry.RegisterSingleton<IHttpClientController, HttpClientController>();

            containerRegistry.Register<ILinkedInService, LinkedInService>();
            containerRegistry.Register<IFourSquareService, FourSquareService>();
            containerRegistry.Register<IGeoLocatorService, GeoLocatorService>();

          

            //Register Xamarin.Essentials Interfaces for DI,

            containerRegistry.Register<IConnectivity,ConnectivityImplementation>();

            //register database services,
            containerRegistry.RegisterSingleton(typeof(DatabaseContext));
            containerRegistry.Register<IVenuesRepository, VenuesRepository>();


            containerRegistry.RegisterSingleton<ISqliteDb, SqliteDb>();
        }
    }
}
