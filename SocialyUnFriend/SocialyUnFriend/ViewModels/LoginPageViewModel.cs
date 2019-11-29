using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using PropertyChanged;
using SocialyUnFriend.Common;
using SocialyUnFriend.Model;
using SocialyUnFriend.NetworkController;
using SocialyUnFriend.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace SocialyUnFriend.ViewModels
{

    public class LoginPageViewModel : BindableBase
    {
        private readonly INavigationService _navigationService;
        private readonly IConnectivity _connectivity;
        private readonly IPageDialogService _pageDialogService;
        private readonly ILinkedInService _linkedInService;
        private readonly IFourSquareService _fourSquareService;
        private readonly IHttpClientController _httpClientController;

        public LoginPageViewModel(INavigationService navigationService, IConnectivity connectivity, IPageDialogService pageDialogService,
                                   ILinkedInService linkedInService, IFourSquareService fourSquareService,
                                   IHttpClientController httpClientController)
        {
            _navigationService = navigationService;
            _connectivity = connectivity;
            _pageDialogService = pageDialogService;
            _linkedInService = linkedInService;
            _fourSquareService = fourSquareService;
            _httpClientController = httpClientController;

           

            NavigationCommand = new DelegateCommand<ImageButtonItem>(OnNavigationCommandExecuted);
            PostCommand = new DelegateCommand(OnPostCommandExecuted);
            OpenCameraCommand = new DelegateCommand(OpenCameraCommandExecuted);

            LoadItems();
        }

        #region DoNotNotify Properties

        [DoNotNotify]
        public DelegateCommand<ImageButtonItem> NavigationCommand { get; set; }

        [DoNotNotify]
        public DelegateCommand PostCommand { get; }

        [DoNotNotify]
        public DelegateCommand OpenCameraCommand { get; }

        #endregion

        #region Fields

        public string checkInID = "";
        public MediaFile file = null;

        #endregion

        #region Notified Properties

        public List<ImageButtonItem> Items { get; set; }

        public string Content { get; set; }
        public string Image { get; set; }

        public bool IsRunning { get; set; }

        #endregion

        #region Public Methods

        public async void OnNavigationCommandExecuted(ImageButtonItem item)
        {
            if (!item.IsEnabled)
                return;

            if (_connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
            {
                await _pageDialogService.DisplayAlertAsync("Network Error!", "Please turn on your internet", "Ok");
                return;
            }

            IsRunning = true;

            //fake delay,,
            await Task.Delay(500);

            var url = OAuthConfig.AuthProviderUrl(item.Platform);

            var navigationParameters = new NavigationParameters
            {
                {"url", url }
            };

            await _navigationService.NavigateAsync("WebViewPage", navigationParameters);


            IsRunning = false;
        }
        public async void OnPostCommandExecuted()
        {

            try
            {
                if (_connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
                {
                    await _pageDialogService.DisplayAlertAsync("Network Error!", "Please turn on your internet", "Ok");
                    return;
                }

                if (string.IsNullOrEmpty(Content) && string.IsNullOrEmpty(Image))
                {
                    await _pageDialogService.DisplayAlertAsync("Alert", "You Can't Share Empty Post", "Ok");
                    return;
                }

                var isTokenAvailable = false;

                if (Application.Current.Properties.ContainsKey(Constants.IsAccessTokenAvailable))
                    isTokenAvailable = (bool)Application.Current.Properties[Constants.IsAccessTokenAvailable];

                if (!isTokenAvailable)
                {
                    await _pageDialogService.DisplayAlertAsync("Please Connect", "You are not connected to any network", "Ok");
                    return;
                }

                IsRunning = true;

                var linkedInToken = "";

                if (Application.Current.Properties.ContainsKey(Constants.AccessTokenLinkedin))
                    linkedInToken = Application.Current.Properties[Constants.AccessTokenLinkedin].ToString();

                var model = await GetUGCPostRequestModel(linkedInToken);

                if (model == null) return;

                var postResponse = await _linkedInService.CreatePost
                    (Constants.LinkedInUGCShareUrl, model, linkedInToken);

                if (postResponse.IsSuccess)
                {
                    Content = string.Empty;
                    Image = string.Empty;

                    await _pageDialogService.DisplayAlertAsync("Post Created", "You just create a post on linked-in.", "Ok");
                }
                else
                {
                    await _pageDialogService.DisplayAlertAsync("Error", postResponse.ErrorMessage, "Ok");
                }


                //if (!string.IsNullOrEmpty(checkInID))
                //{

                //    if (!string.IsNullOrEmpty(Image))
                //    {
                //        ApiResponse<object> response = null;

                //        response = await _fourSquareService.AddPhoto(Constants.FSAddPhotoURL, token, checkInID,
                //                                                     ImageHelper.UriPathToBytes(Image), Content,
                //                                                     DateTime.Now.ToString("yyyyMMdd"));


                //        if (response.IsSuccess)
                //        {
                //            await _pageDialogService.DisplayAlertAsync("Message!", "Photos Upload at you Checkin.", "Ok");

                //            Content = string.Empty;
                //            Image = string.Empty;
                //        }
                //        else
                //        {
                //            await _pageDialogService.DisplayAlertAsync("Error!", "Something Went wrong, please try again later", "Ok");
                //        }
                //    }
                //    else if (!string.IsNullOrEmpty(Content) && string.IsNullOrEmpty(Image))
                //    {
                //        if (Content.Length > 200)
                //        {
                //            await _pageDialogService.DisplayAlertAsync("Warning!", "Content should not be greater than 200 characters", "Ok");
                //            return;
                //        }
                //        var response = await _fourSquareService.AddCheckinPost(Constants.FSCheckInPostURL, token, checkInID, Content, DateTime.Now.ToString("yyyyMMdd"));

                //        if (response.IsSuccess)
                //        {
                //            await _pageDialogService.DisplayAlertAsync("Message!", "Your post has been created", "Ok");

                //            Content = string.Empty;

                //        }
                //        else
                //        {
                //            await _pageDialogService.DisplayAlertAsync("Error!", "Something Went wrong, please try again later", "Ok");
                //        }
                //    }
                //    else
                //    {
                //        await _pageDialogService.DisplayAlertAsync("Message!", "Please enter some text or take selfie to upload.", "Ok");

                //    }

                //    return;
                //}


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
        public async void OpenCameraCommandExecuted()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync<PhotosPermission>();
                if (status != PermissionStatus.Granted)
                {
                    status = await CrossPermissions.Current.RequestPermissionAsync<PhotosPermission>();
                }

                if (status == PermissionStatus.Granted)
                {
                    await TakeOrPickPictures();
                }
                else if (status != PermissionStatus.Unknown)
                {
                    //permission denied
                    await _pageDialogService.DisplayAlertAsync("Denied!", "Camera Permission is required", "OK");
                }



            }
            catch (Exception)
            {

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
                    IsEnabled = true
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

                foreach (var item in items)
                {
                    if (Application.Current.Properties.ContainsKey(Constants.IsLinkedInConnected))
                    {
                        if ((bool)Application.Current.Properties[Constants.IsLinkedInConnected] && item.Platform == SocialMediaPlatform.LinkedIn)
                        {

                            item.IsEnabled = false;
                            item.Text = "Connected to Linkedin";
                            item.Color = Color.Gray;
                        }
                        

                        if (Application.Current.Properties.ContainsKey(Constants.IsFourSquareConnected) && item.Platform == SocialMediaPlatform.FourSquare)
                        {
                            if ((bool)Application.Current.Properties[Constants.IsFourSquareConnected])
                            {
                                item.IsEnabled = false;
                                item.Text = "Connected to FourSquare";
                                item.Color = Color.Gray;
                            }
                        }

                    }
                }

                Items = new List<ImageButtonItem>(items);

            }
            catch (Exception ex)
            {

            }
        }

        private async Task TakeOrPickPictures()
        {
            try
            {
                var result = await _pageDialogService.DisplayActionSheetAsync("Choose", null, null, "Take Selfie", "Pick Photos", "Cancel");

                await CrossMedia.Current.Initialize();

                if (result == "Take Selfie")
                {

                    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        await _pageDialogService.DisplayAlertAsync("No Camera", ":( No camera available.", "OK");
                        return;
                    }

                    file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                    {
                        Directory = "Test",
                        SaveToAlbum = true,
                        CompressionQuality = 75,
                        SaveMetaData = true,
                        PhotoSize = PhotoSize.Large,
                        DefaultCamera = CameraDevice.Front

                    });

                    if (file == null)
                    {
                        return;
                    }

                    Image = file.Path;
                }
                else if (result == "Pick Photos")
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync<StoragePermission>();
                    if (status != PermissionStatus.Granted)
                    {
                        status = await CrossPermissions.Current.RequestPermissionAsync<StoragePermission>();
                    }

                    if (status == PermissionStatus.Granted)
                    {
                        file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                        {
                            CompressionQuality = 75,
                            PhotoSize = PhotoSize.Large,
                            SaveMetaData = true
                        });

                        if (file == null)
                        {
                            return;
                        }

                        Image = file.Path;

                    }
                    else if (status != PermissionStatus.Unknown)
                    {
                        //permission denied
                        await _pageDialogService.DisplayAlertAsync("Denied!", "Storage Permission is required", "OK");
                    }

                }
                else
                {

                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (file != null)
                    file.Dispose();
            }
        }
        private async Task<UGCPostRequestModel> GetUGCPostRequestModel(string linkedInToken)
        {
            try
            {
                UGCPostRequestModel uGCPostRequestModel = null;

                string urnAssestForImageSharing = "";

                if (!string.IsNullOrEmpty(Image))
                {
                    var regiterUploadModel = new RegisterUpload();
                    regiterUploadModel.registerUploadRequest = new RegisterUploadRequest
                    {
                        recipes = new List<string>()
                        {
                               {
                                    @"urn:li:digitalmediaRecipe:feedshare-image"
                               }
                        }
                    };


                    regiterUploadModel.registerUploadRequest.owner = Constants.UrnOwner + Application.Current.Properties["userID"].ToString();

                    regiterUploadModel.registerUploadRequest.serviceRelationships = new List<ServiceRelationship>
                {
                    new ServiceRelationship { relationshipType = "OWNER", identifier = "urn:li:userGeneratedContent" }
                };

                    var response = await _linkedInService.RegisterUpload(Constants.LinkedInRegisterUploadUrl,
                                                                         regiterUploadModel,
                                                                         linkedInToken);

                    if (response.IsSuccess)
                    {
                        var data = JsonConvert.DeserializeObject<RegisterUploadResponse>(response.ResultData.ToString());

                        var uploadUrl = data.value.uploadMechanism.MediaUploadHttpRequest.UpLoadUrl;

                        urnAssestForImageSharing = data.value.asset;

                        await _httpClientController.UploadImage(uploadUrl, ImageHelper.UriPathToBytes(Image),
                                                                linkedInToken);

                        uGCPostRequestModel = await UGCShareTextWithImageModel(urnAssestForImageSharing, linkedInToken);
                    }
                }
                else
                {
                    uGCPostRequestModel = UGCShareTextModel();
                }


                return uGCPostRequestModel;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private UGCPostRequestModel UGCShareTextModel()
        {
            var shareTextModel = new UGCPostRequestModel
            {
                author = Constants.UrnOwner + Application.Current.Properties["userID"].ToString(),
                lifecycleState = "PUBLISHED",
                specificContent = new SpecificContent
                {
                    ShareContent = new ComLinkedinUgcShareContent
                    {
                        shareCommentary = new ShareCommentary
                        {
                            text = Content
                        },

                        shareMediaCategory = "NONE",

                        media = new List<Medium>()

                    }
                },

                visibility = new Visibility { MemberNetworkVisibility = "PUBLIC" }
            };

            return shareTextModel;
        }

        private async Task<UGCPostRequestModel> UGCShareTextWithImageModel(string urnAssets, string token)
        {
            try
            {
                var userProfileId = await GetUserProfileId(token);

                var model = new UGCPostRequestModel
                {
                    author = Constants.UrnOwner + userProfileId,
                    lifecycleState = "PUBLISHED",
                    specificContent = new SpecificContent
                    {
                        ShareContent = new ComLinkedinUgcShareContent
                        {
                            shareCommentary = new ShareCommentary
                            {
                                text = string.IsNullOrEmpty(Content) ? "  " : Content
                            },

                            shareMediaCategory = "IMAGE",

                            media = new List<Medium>
                        {
                            new Medium
                            {
                                status = "READY" ,
                                description = new Description
                                {
                                    text = "Description of Image"
                                },

                                media = urnAssets,

                                title = new Title
                                {
                                    text = "title of image"
                                }
                            }
                        }

                        }
                    },

                    visibility = new Visibility { MemberNetworkVisibility = "PUBLIC" }
                };

                return model;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<string> GetUserProfileId(string token)
        {
            string userId = "";
            var apiResponse = await _linkedInService.GetUserProfile
                         (
                          Constants.LinkedInProfileUrl, token
                         );

            if (apiResponse.IsSuccess)
            {
                var data = apiResponse.ResultData as LinkedInProfile;
                Application.Current.Properties[Constants.LinkedInUserId] = userId = data.UserProfileID;
                await Application.Current.SavePropertiesAsync();
            }


            return userId;
        }

        #endregion
    }
}
