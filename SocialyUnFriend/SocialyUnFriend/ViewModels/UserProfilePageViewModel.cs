﻿using Prism.Commands;
using Prism.Mvvm;
using SocialyUnFriend.Common;
using SocialyUnFriend.Services;
using System;
using SocialyUnFriend.Model;
using Xamarin.Forms;
using System.Linq;
using Prism.Navigation;
using SocialyUnFriend.Views;
using Microsoft.AppCenter.Crashes;
using System.Collections.Generic;
using Xamarin.Essentials.Interfaces;
using Prism.Services;
using Xamarin.Essentials;

namespace SocialyUnFriend.ViewModels
{
    public class UserProfilePageViewModel : BindableBase, INavigationAware
    {
        private string photoSize = "400x400";
        private readonly ILinkedInService _linkedInService;
        private readonly IFourSquareService _fourSquareService;
        private readonly INavigationService _navigationService;
        private readonly IConnectivity _connectivity;
        private readonly IPageDialogService _pageDialogService;

        public UserProfilePageViewModel(ILinkedInService linkedInService, IFourSquareService fourSquareService,
                                        INavigationService navigationService, IConnectivity connectivity,IPageDialogService pageDialogService)
        {
            _linkedInService = linkedInService;
            _fourSquareService = fourSquareService;
            _navigationService = navigationService;
            _connectivity = connectivity;
            _pageDialogService = pageDialogService;


            NavigateCommand = new DelegateCommand<string>(NavigateCommandExecuted);

            _connectivity.ConnectivityChanged += _connectivity_ConnectivityChanged;
        }

        ~UserProfilePageViewModel()
        {
            _connectivity.ConnectivityChanged -= _connectivity_ConnectivityChanged;
        }

        private async void _connectivity_ConnectivityChanged(object sender, ConnectivityChangedEventArgs e)
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                await _pageDialogService.DisplayAlertAsync("Connection Error", "Internet is not available.", "Ok");
            else
            {
                if (IsPostBtnVisible)
                    LoadLinkedInUserProfile();
                else
                    LoadFourSquareUserProfile();
            }
        }

        public DelegateCommand<string> NavigateCommand { get; }


        public async void NavigateCommandExecuted(string pageName)
        {
            
            await _navigationService.NavigateAsync(pageName);
           
        }


        private string _image;
        public string Image
        {
            get { return _image; }
            set { SetProperty(ref _image, value); }
        }

        private string _profileId;
        public string ProfileId
        {
            get { return _profileId; }
            set { SetProperty(ref _profileId, value); }
        }
        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set { SetProperty(ref _firstName, value); }
        }
        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set { SetProperty(ref _lastName, value); }
        }


        private bool _isPostBtnVisible = false;
        public bool IsPostBtnVisible
        {
            get { return _isPostBtnVisible; }
            set { SetProperty(ref _isPostBtnVisible, value); }
        }

        private bool _isCheckInBtnVisible = false;
        public bool IsCheckInBtnVisible
        {
            get { return _isCheckInBtnVisible; }
            set { SetProperty(ref _isCheckInBtnVisible, value); }
        }



        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set { SetProperty(ref _isRunning, value); }
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (Application.Current.Properties.ContainsKey("LoginAs"))
            {
                var platForm = Convert.ToInt32(Application.Current.Properties["LoginAs"]);

                if (platForm == Convert.ToInt32(SocialMediaPlatform.LinkedIn))
                {
                    IsPostBtnVisible = true;
                    LoadLinkedInUserProfile();
                }
                else if (platForm == Convert.ToInt32(SocialMediaPlatform.FourSquare))
                {
                    IsCheckInBtnVisible = true;
                    LoadFourSquareUserProfile();
                }
            }
        }

        private async void LoadLinkedInUserProfile()
        {
            try
            {
               

                if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await _pageDialogService.DisplayAlertAsync("Network Error!", "Please turn on your internet", "Ok");
                    });

                    return;
                }

                IsRunning = true;

                var apiResponse = await _linkedInService.GetUserProfile
                    (
                    Constants.LinkedInProfileUrl, (string)Application.Current.Properties["acces_token"]
                    );

                if (apiResponse.IsSuccess)
                {
                    var model = apiResponse.ResultData as LinkedInProfile;
                    var image = model.UserProfilePicture.DisplayImage.Elements.Last().Identifiers.First().identifier;

                    Image = image;
                    ProfileId = model.UserProfileID;
                    FirstName = model.FirstName;
                    LastName = model.LastName;

                    Application.Current.Properties["userID"] = ProfileId;
                    await Application.Current.SavePropertiesAsync();
                }

            }
            catch (Exception)
            {
            }
            finally
            {
                IsRunning = false;
            }
        }

        private async void LoadFourSquareUserProfile()
        {
            try
            {


                if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    Device.BeginInvokeOnMainThread(async ()  =>
                    {
                        await _pageDialogService.DisplayAlertAsync("Network Error!", "Please turn on your internet", "Ok");
                    });
                   

                    return;
                }

                IsRunning = true;

                var apiResponse = await _fourSquareService.GetUserProfile
                    (
                    Constants.FSUserProfileURL, Application.Current.Properties["acces_token"].ToString(), DateTime.Now.ToString("yyyyMMdd")
                    );

                if (apiResponse.IsSuccess)
                {
                    var model = apiResponse.ResultData;
                    var photoPrefix = model.response.user.photo.prefix;
                    var photoSuffix = model.response.user.photo.suffix;

                    Image = photoPrefix + photoSize + photoSuffix;

                    ProfileId = model.response.user.id;
                    FirstName = model.response.user.firstName;
                    LastName = model.response.user.lastName;

                    Application.Current.Properties["fourSquareProfileId"] = ProfileId;
                    await Application.Current.SavePropertiesAsync();
                }

                
            }
            catch (Exception exception)
            {

                var properties = new Dictionary<string, string>
                    {
                        { "Category", "User Profile" },
                        { "Wifi", "On"}
                    };
                Crashes.TrackError(exception, properties);
            }
            finally
            {
                IsRunning = false;
            }
        }
    }
}
