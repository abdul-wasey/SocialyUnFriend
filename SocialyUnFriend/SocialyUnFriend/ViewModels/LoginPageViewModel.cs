using Microsoft.AppCenter.Crashes;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using SocialyUnFriend.Common;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;

namespace SocialyUnFriend.ViewModels
{
    public class LoginPageViewModel : BindableBase
    {
        private readonly INavigationService _navigationService;
        private readonly IConnectivity _connectivity;

        public LoginPageViewModel(INavigationService navigationService, IConnectivity connectivity)
        {
            _navigationService = navigationService;
            _connectivity = connectivity;


            NavigationCommand = new DelegateCommand<string>(OnNavigationCommandExecuted);
            
        }

        public DelegateCommand<string> NavigationCommand { get; }

        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set { SetProperty(ref _isRunning, value); }
        }

        private async void OnNavigationCommandExecuted(string platform)
        {
            string url = "";

            IsRunning = true;
            //fake delay,,
            await Task.Delay(1000);

            Crashes.GenerateTestCrash();

            if (platform == "linkedin")
                url = OAuthConfig.AuthProviderUrl(SocialMediaPlatform.LinkedIn);
            else if(platform == "foursquare")
                url = OAuthConfig.AuthProviderUrl(SocialMediaPlatform.FourSquare);

            var navigationParameters = new NavigationParameters
            {
                {"url", url }
            };

            await _navigationService.NavigateAsync("WebViewPage", navigationParameters);
            

            IsRunning = false;
        }
    }
}
