using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using SocialyUnFriend.Common;
using SocialyUnFriend.Model;
using SocialyUnFriend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace SocialyUnFriend.ViewModels
{
    public class LinkedInPostPageViewModel : BindableBase, INavigationAware
    {
        private readonly IPageDialogService _pageDialogService;
        private readonly ILinkedInService _linkedInService;
        private readonly IFourSquareService _fourSquareService;
        private readonly IConnectivity _connectivity;


        public LinkedInPostPageViewModel(IPageDialogService pageDialogService, ILinkedInService linkedInService, IFourSquareService fourSquareService,
                                         IConnectivity connectivity)
        {
            _pageDialogService = pageDialogService;
            _linkedInService = linkedInService;
            _fourSquareService = fourSquareService;
            _connectivity = connectivity;


            PostCommand = new DelegateCommand(OnPostCommandExecuted);

        }

        public DelegateCommand PostCommand { get; }

        public string checkInID = "";

        private string _content;
        public string Content
        {
            get { return _content; }
            set { SetProperty(ref _content, value); }
        }

        private bool _isRunning;
        public bool IsRunning
        {
            get { return _isRunning; }
            set { SetProperty(ref _isRunning, value); }
        }

        


        public IPageDialogService PageDialogService { get; }

        private async void OnPostCommandExecuted()
        {

            try
            {
                if (_connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
                {
                    await _pageDialogService.DisplayAlertAsync("Network Error!", "Please turn on your internet", "Ok");

                    return;
                }

                IsRunning = true;

                var token = "";

                if (Application.Current.Properties.ContainsKey("acces_token"))
                    token = Application.Current.Properties["acces_token"].ToString();

                if (!string.IsNullOrEmpty(checkInID))
                {
                    if (Content.Length > 200)
                    {
                        await _pageDialogService.DisplayAlertAsync("Warning!", "Content should be up to 200 characters", "Ok");
                        return;
                    }

                    var response = await _fourSquareService.AddCheckinPost(Constants.FSCheckInPostURL, token, checkInID, Content, DateTime.Now.ToString("yyyyMMdd"));

                    if (response.IsSuccess)
                    {
                        await _pageDialogService.DisplayAlertAsync("Message!", "Post Created..!", "Ok");

                        Content = string.Empty;
                    }
                    else
                    {
                        await _pageDialogService.DisplayAlertAsync("Error!", response.ErrorMessage, "Ok");
                    }

                    return;
                }

                var model = GetLinkedInPostModel();

                var postResponse = await _linkedInService.CreatePost
                    (Constants.LinkedInPostShareUrl, model, token);

                if (postResponse.IsSuccess)
                {
                    var data = JsonConvert.DeserializeObject<LinkedInPostShareResponse>(postResponse.ResultData.ToString());

                    await _pageDialogService.DisplayAlertAsync("Post Created", $"{data.owner} created a post.", "Ok");
                }
                else
                {
                    await _pageDialogService.DisplayAlertAsync("Error", "Something went wrong", "Ok");
                }
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
            }
            finally
            {
                IsRunning = false;
            }

        }


        private LinkedInPost GetLinkedInPostModel()
        {
            var model = new LinkedInPost
            {
                Content = new Content { Title = "Testing Share Api" }
            };
            var entities = new List<ContentEntity>
            {
                new ContentEntity { EntityLocation = "https://www.google.com/" }

            };
            var thumbs = new List<Thumbnail>
            {
                new Thumbnail { ResolvedUrl = "https://www.google.com/" }
            };

            model.Content.ContentEntities = entities;
            model.Content.ContentEntities.First().Thumbnails = thumbs;

            model.Distribution = new Distribution
            {
                LinkedInDistributionTarget = new LinkedInDistributionTarget()
            };

            if(Application.Current.Properties.ContainsKey("userID"))
                model.Owner = Constants.UrnOwner + (string)Application.Current.Properties["userID"];

            model.Subject = "Example subject of share api";
            model.Text = new Text { text = Content };

            return model;
        }

        public void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("checkInId"))
            {
                checkInID = parameters.GetValue<string>("checkInId");
            }
        }
    }
}
