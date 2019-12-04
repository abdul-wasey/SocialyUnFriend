using DryIoc;
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

            // Register Services,,
           
            containerRegistry.RegisterSingleton<IHttpClientController, HttpClientController>();

            containerRegistry.Register<ILinkedInService, LinkedInService>();
            containerRegistry.Register<IFourSquareService, FourSquareService>();
            containerRegistry.Register<IGeoLocatorService, GeoLocatorService>();

            containerRegistry.Register<IVenuesRepository, VenuesRepository>();

            //Register Xamarin.Essentials Interfaces for DI,

            containerRegistry.Register<IConnectivity,ConnectivityImplementation>();

            containerRegistry.RegisterSingleton(typeof(DatabaseContext));
        }
    }
}
