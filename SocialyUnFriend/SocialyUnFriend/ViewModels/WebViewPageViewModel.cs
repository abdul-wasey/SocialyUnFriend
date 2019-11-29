using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SocialyUnFriend.Common;
using SocialyUnFriend.Services;
using System;
using SocialyUnFriend.NetworkController;
using System.Web;
using Xamarin.Forms;
using SocialyUnFriend.Model;
using PropertyChanged;

namespace SocialyUnFriend.ViewModels
{
    public class WebViewPageViewModel : BindableBase, INavigationAware
    {
        private readonly IHttpClientController _httpClientController;
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _pageDialogService;

        public WebViewPageViewModel(IHttpClientController httpClientController, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            _httpClientController = httpClientController;
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;

            WebViewNavigatingCommand = new DelegateCommand<string>(OnNavigating);
            
        }

        [DoNotNotify]
        public DelegateCommand<string> WebViewNavigatingCommand { get; }


        public string Url { get; set; }
      


        public void OnNavigatedFrom(INavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("url"))
            {
                Url = parameters.GetValue<string>("url");
            }
        }

        private async void OnNavigating(string url)
        {
            try
            {
                Uri uri = new Uri(url);

                bool isLinkedInRedirectUri = uri.AbsolutePath.ToLower().Contains("oauth-success");
                bool isFourSquareRedirectUri = uri.AbsolutePath.ToLower().Contains("announcements");

                if (isLinkedInRedirectUri || isFourSquareRedirectUri)
                {
                    //parse the response
                    var code = HttpUtility.ParseQueryString(uri.Query).Get("code");
                    var error = HttpUtility.ParseQueryString(uri.Query).Get("error");


                    if (error != null)
                    {
                        await _pageDialogService.DisplayAlertAsync("Error", "There was trouble logging you in", "Try Again");
                        return;
                    }


                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        string accessTokenUrl = "";
                        ApiResponse<OAuthToken> tokenResponse = new ApiResponse<OAuthToken>();

                        if (isLinkedInRedirectUri)
                        {
                            accessTokenUrl = OAuthConfig.AccessTokenUrl(SocialMediaPlatform.LinkedIn);
                            tokenResponse = await _httpClientController.GetOAuthTokenAsync
                                                      (accessTokenUrl, code, Constants.ClientID, Constants.ClientSecret, Constants.RedirectURI);

                        }
                        else if (isFourSquareRedirectUri)
                        {
                            accessTokenUrl = OAuthConfig.AccessTokenUrl(SocialMediaPlatform.FourSquare);
                            tokenResponse = await _httpClientController.GetOAuthTokenAsync
                                                       (accessTokenUrl, code, Constants.FSClientID, Constants.FSClientSecret, Constants.FSRedirectLink);
                        }

                        if (tokenResponse.IsSuccess)
                        {
                            if (isLinkedInRedirectUri)
                            {
                                Application.Current.Properties[Constants.AccessTokenLinkedin] = tokenResponse.ResultData.AccessToken;
                                Application.Current.Properties[Constants.AccessTokenLinkedinExpiryDate] = tokenResponse.ResultData.ExpireDate;
                                Application.Current.Properties[Constants.IsAccessTokenAvailable] = true;
                                Application.Current.Properties[Constants.IsLinkedInConnected] = true;

                            }
                            else if (isFourSquareRedirectUri)
                            {
                                Application.Current.Properties[Constants.AccessTokenFourSquare] = tokenResponse.ResultData.AccessToken;
                                Application.Current.Properties[Constants.IsAccessTokenAvailable] = true;
                                Application.Current.Properties[Constants.IsFourSquareConnected] = true;
                            }

                            await Application.Current.SavePropertiesAsync();
                        }
                        else
                            await _pageDialogService.DisplayAlertAsync("Error", tokenResponse.ErrorMessage, "Ok");


                        await _navigationService.NavigateAsync("/NavigationPage/LoginPage");
                    });
                }

            }
            catch (Exception)
            {

            }
        }
    }
}
