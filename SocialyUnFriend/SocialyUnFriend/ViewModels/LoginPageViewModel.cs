using Microsoft.AppCenter.Crashes;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SocialyUnFriend.Common;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;

namespace SocialyUnFriend.ViewModels
{
    public class LoginPageViewModel : BindableBase
    {
        private readonly INavigationService _navigationService;
        private readonly IConnectivity _connectivity;
        private readonly IPageDialogService _pageDialogService;

        public LoginPageViewModel(INavigationService navigationService, IConnectivity connectivity, IPageDialogService pageDialogService)
        {
            _navigationService = navigationService;
            _connectivity = connectivity;
            _pageDialogService = pageDialogService;


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
            if(_connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                await _pageDialogService.DisplayAlertAsync("Network Error!", "Please turn on your internet", "Ok");

                return;
            }

            string url = "";

            IsRunning = true;
            //fake delay,,
            await Task.Delay(1000);


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
