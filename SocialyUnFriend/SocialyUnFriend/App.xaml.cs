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


            if (Current.Properties.ContainsKey("acces_token") && (string)Current.Properties["acces_token"] != null)
                await NavigationService.NavigateAsync("NavigationPage/UserProfilePage");
            else
                //MainPage = new NavigationPage(new LoginPage());
                await NavigationService.NavigateAsync("NavigationPage/WelcomePage");

        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();

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
