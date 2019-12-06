using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.AppModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Prism.Services.Dialogs;
using PropertyChanged;
using SocialyUnFriend.Common;
using SocialyUnFriend.Model;
using SocialyUnFriend.NetworkController;
using SocialyUnFriend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace SocialyUnFriend.ViewModels
{

    public class LoginPageViewModel : BindableBase, IPageLifecycleAware
    {
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IConnectivity _connectivity;
        private readonly IPageDialogService _pageDialogService;
        private readonly ILinkedInService _linkedInService;
        private readonly IFourSquareService _fourSquareService;
        private readonly IHttpClientController _httpClientController;
        private readonly IGeoLocatorService _geoLocatorService;

        public LoginPageViewModel(INavigationService navigationService, IConnectivity connectivity, IPageDialogService pageDialogService,
                                   ILinkedInService linkedInService, IFourSquareService fourSquareService,
                                   IHttpClientController httpClientController, IGeoLocatorService geoLocatorService,
                                   IDialogService dialogService)
        {
            _navigationService = navigationService;
            _connectivity = connectivity;
            _pageDialogService = pageDialogService;
            _linkedInService = linkedInService;
            _fourSquareService = fourSquareService;
            _httpClientController = httpClientController;
            _geoLocatorService = geoLocatorService;
            _dialogService = dialogService;

            NavigationCommand = new DelegateCommand<ImageButtonItem>(OnNavigationCommandExecuted);
            OpenDialogCommand = new DelegateCommand(OpenDialogCommandExecuted);
            RecentPostsCommand = new DelegateCommand(RecentPostsCommandExecuted);
        }

        #region DoNotNotify Properties

        [DoNotNotify]
        public DelegateCommand<ImageButtonItem> NavigationCommand { get; }

        [DoNotNotify]
        public DelegateCommand RecentPostsCommand { get; }

        [DoNotNotify]
        public DelegateCommand OpenDialogCommand { get; }

        #endregion

        #region Fields

        public bool IsLinkedInConnected = false;
        public bool IsFourSquareConnected = false;

        #endregion

        #region Notified Properties

        public List<ImageButtonItem> Items { get; set; }

        public string LoaderText { get; set; } = "Loading...";

        public bool IsRunning { get; set; }

        #endregion

        #region Public Methods


        public void OnAppearing()
        {
            LoadItems();
        }

        public void OnDisappearing()
        {

        }

        public async void OpenDialogCommandExecuted()
        {
            // Required Permissions ,, 
            try
            {

                var photosPermissionStatus = await Utils.CheckPermissions<PhotosPermission>(Permission.Photos);
                var storagePermissionStatus = await Utils.CheckPermissions<StoragePermission>(Permission.Storage);
                var locationPermissionStatus = await Utils.CheckPermissions<LocationPermission>(Permission.Location);

                if (locationPermissionStatus == PermissionStatus.Granted)
                {
                    if (Application.Current.Properties.ContainsKey(Constants.IsLinkedInConnected))
                    {
                        if ((bool)Application.Current.Properties[Constants.IsLinkedInConnected])
                            IsLinkedInConnected = true;
                        else
                            IsLinkedInConnected = false;
                    }
                    if (Application.Current.Properties.ContainsKey(Constants.IsFourSquareConnected))
                    {
                        if ((bool)Application.Current.Properties[Constants.IsFourSquareConnected])
                            IsFourSquareConnected = true;
                        else
                            IsFourSquareConnected = false;

                    }

                    if (!IsLinkedInConnected && !IsFourSquareConnected)
                    {
                        await _pageDialogService.DisplayAlertAsync("Message", "Please connect with any network first", "Ok");
                        return;
                    }

                    var paras = new DialogParameters
                    {
                        {"IsLinkedInChecked", IsLinkedInConnected },
                        {"IsFourSquareChecked", IsFourSquareConnected }
                    };

                    _dialogService.ShowDialog("PostDialog", paras, CloseDialog);
                }
                else
                {
                    await _pageDialogService.DisplayAlertAsync("Message", "You can not proceed without allowing permissions", "Ok");
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }

        }

        public async void RecentPostsCommandExecuted()
        {
            await _navigationService.NavigateAsync("GetVenuesListPage");
        }

        public static void CloseDialog(IDialogResult dialogResult)
        {

        }
        public async void OnNavigationCommandExecuted(ImageButtonItem item)
        {
            try
            {
                if (_connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
                {
                    await _pageDialogService.DisplayAlertAsync("Network Error!", "Please turn on your internet", "Ok");
                    return;
                }

                var url = OAuthConfig.AuthProviderUrl(item.Platform);

                IsRunning = true;

                //fake delay,,
                await Task.Delay(500);

                if (!item.IsEnabled)
                {
                    if (item.Platform == SocialMediaPlatform.FourSquare)
                        url = Constants.FSConnectedAppsURL;
                    else if (item.Platform == SocialMediaPlatform.LinkedIn)
                        url = Constants.LinkedInPermittedServicesUrl;
                }

                var navigationParameters = new NavigationParameters
                {
                    {"url", url }
                };

                await _navigationService.NavigateAsync("WebViewPage", navigationParameters);

            }
            catch (Exception)
            {

            }
            finally
            {
                IsRunning = false;
            }
        }

        #endregion

        #region Private Methods

        private void LoadItems()
        {

            try
            {

                var items = new List<ImageButtonItem>
                {
                    new ImageButtonItem
                    {

                        ImageSource = "linkedin_white_icon.jpg",
                        Text = "Login With Linkedin",
                        Color = Color.FromHex("#00A9F4"),
                        Platform = SocialMediaPlatform.LinkedIn,
                        IsEnabled  = true
                    },
                    new ImageButtonItem
                    {
                        ImageSource = "foursquare_icon.png",
                        Text = "Login With FourSquare",
                        Color = Color.FromHex("#FA4678"),
                        Platform = SocialMediaPlatform.FourSquare,
                        IsEnabled = true
                    }
                };

                if (Application.Current.Properties.ContainsKey(Constants.IsLinkedInConnected) && (bool)Application.Current.Properties[Constants.IsLinkedInConnected])
                {
                    var linkedInBtn = items.Where(x => x.Platform == SocialMediaPlatform.LinkedIn).SingleOrDefault();

                    if (linkedInBtn != null)
                    {
                        linkedInBtn.IsEnabled = false;
                        linkedInBtn.Text = "Connected to Linkedin";
                        linkedInBtn.Color = Color.Gray;
                    }
                }

                if (Application.Current.Properties.ContainsKey(Constants.IsFourSquareConnected) && (bool)Application.Current.Properties[Constants.IsFourSquareConnected])
                {
                    var fourSquareBtn = items.Where(x => x.Platform == SocialMediaPlatform.FourSquare).SingleOrDefault();
                    if (fourSquareBtn != null)
                    {
                        fourSquareBtn.IsEnabled = false;
                        fourSquareBtn.Text = "Connected to FourSquare";
                        fourSquareBtn.Color = Color.Gray;
                    }
                }


                Items = new List<ImageButtonItem>(items);

            }
            catch (Exception)
            {

            }
        }

        #endregion
    }
}
