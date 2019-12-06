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
using SocialyUnFriend.DependencyServcices;
using SocialyUnFriend.LocalDB;
using SocialyUnFriend.Model;
using SocialyUnFriend.NetworkController;
using SocialyUnFriend.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials.Interfaces;
using Xamarin.Forms;

namespace SocialyUnFriend.ViewModels
{
    public class PostDialogViewModel : BindableBase, IDialogAware, IAutoInitialize
    {
      
        private readonly IConnectivity _connectivity;
        private readonly IPageDialogService _pageDialogService;
        private readonly ILinkedInService _linkedInService;
        private readonly IFourSquareService _fourSquareService;
        private readonly IHttpClientController _httpClientController;
        private readonly IGeoLocatorService _geoLocatorService;
        private readonly ILocationSettings _locationSettings;
        private readonly ISqliteDb _sqliteDb;

        public PostDialogViewModel(IConnectivity connectivity, IPageDialogService pageDialogService,
                                   ILinkedInService linkedInService, IFourSquareService fourSquareService,
                                   IHttpClientController httpClientController, IGeoLocatorService geoLocatorService,
                                   ILocationSettings locationSettings, ISqliteDb sqliteDb)
                                   
        {
            
            _connectivity = connectivity;
            _pageDialogService = pageDialogService;
            _linkedInService = linkedInService;
            _fourSquareService = fourSquareService;
            _httpClientController = httpClientController;
            _geoLocatorService = geoLocatorService;
            _locationSettings = locationSettings;
            _sqliteDb = sqliteDb;

            CloseCommand = new DelegateCommand(() => RequestClose(null));
            PostCommand = new DelegateCommand(OnPostCommandExecuted);
            OpenCameraCommand = new DelegateCommand(OpenCameraCommandExecuted);
        }

        [DoNotNotify]
        public DelegateCommand PostCommand { get; }

        [DoNotNotify]
        public DelegateCommand OpenCameraCommand { get; }
        #region DelegateCommands 

        [DoNotNotify]
        public DelegateCommand CloseCommand { get; }

        public event Action<IDialogParameters> RequestClose;

        public bool CanCloseDialog() => true;
        #endregion


        #region Notified Properties

      
        public bool IsLinkedInChecked { get; set; }
        public bool IsFourSquareChecked { get; set; }

        public string Content { get; set; }
        public string LoaderText { get; set; } = "Loading...";
        public string Image { get; set; }

        public bool IsRunning { get; set; }

        #endregion


        #region Fields

        public string venueID = "";
        public MediaFile file = null;

        #endregion
      

        #region Public Methods

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {

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


                var fourSquareToken = "";
                if (Application.Current.Properties.ContainsKey(Constants.AccessTokenFourSquare))
                    fourSquareToken = Application.Current.Properties[Constants.AccessTokenFourSquare].ToString();

                if (IsFourSquareChecked && !string.IsNullOrEmpty(fourSquareToken))
                {
                    if(!_geoLocatorService.IsGpsEnabled())
                    {
                       var acceptButton = await _pageDialogService.DisplayAlertAsync("Message", "You can't proceed without turn on GPS", "Proceed","Maybe Later");
                        if(acceptButton)
                            _locationSettings.OpenSettings();

                        return;
                    }

                    LoaderText = "Posting on FourSquare...";

                    string venueId = await CurrentCheckin(fourSquareToken);

                    if (!string.IsNullOrEmpty(venueId))
                    {
                        if (!string.IsNullOrEmpty(Image))
                        {
                            ApiResponse<object> response = null;

                            response = await _fourSquareService.AddPhoto(Constants.FSAddPhotoURL, fourSquareToken, venueId,
                                                                         ImageHelper.UriPathToBytes(Image), Content,
                                                                         DateTime.Now.ToString("yyyyMMdd"));
                            if (response.IsSuccess)
                            {
                                var recentPostModel = new RecentPost
                                {
                                    Text = Content,
                                    ImageUri = Image,
                                    Platform = SocialMediaPlatform.FourSquare.ToString(),
                                    DateTime = DateTime.Now
                                };
                                
                                await _sqliteDb.SaveAsync(recentPostModel);
                                
                            }
                            else
                            {
                                await _pageDialogService.DisplayAlertAsync("Error", response.ErrorMessage, "Ok");
                            }
                        }
                    }

                }
                else if(IsFourSquareChecked && string.IsNullOrEmpty(fourSquareToken))
                {
                    await _pageDialogService.DisplayAlertAsync("Message", "You are not connected to Foursquare", "Ok");
                }
               

                var linkedInToken = "";

                if (Application.Current.Properties.ContainsKey(Constants.AccessTokenLinkedin))
                    linkedInToken = Application.Current.Properties[Constants.AccessTokenLinkedin].ToString();

                if (IsLinkedInChecked && !string.IsNullOrEmpty(linkedInToken))
                {
                    var model = await GetUGCPostRequestModel(linkedInToken);

                    if (model == null) return;

                    LoaderText = "Posting on Linkedin...";

                    var postResponse = await _linkedInService.CreatePost
                        (Constants.LinkedInUGCShareUrl, model, linkedInToken);

                    if (postResponse.IsSuccess)
                    {
                        var recentPostModel = new RecentPost
                        {
                            Text = Content,
                            ImageUri = Image,
                            Platform = $"Posted on {SocialMediaPlatform.LinkedIn}",
                            DateTime = DateTime.Now
                        };

                        await _sqliteDb.SaveAsync(recentPostModel);
                    }
                    else
                    {
                        await _pageDialogService.DisplayAlertAsync("Error", postResponse.ErrorMessage, "Ok");
                    }

                }
                else if (IsLinkedInChecked && string.IsNullOrEmpty(linkedInToken))
                {
                    await _pageDialogService.DisplayAlertAsync("Message", "You are not connected to Linkedin", "Ok");
                }


                if (!IsFourSquareChecked && !IsLinkedInChecked)
                {
                    await _pageDialogService.DisplayAlertAsync("Message!", "Please Check desired Platform to Proceed.", "Ok");
                    return;
                }

                Content = string.Empty;
                Image = string.Empty;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);

                Content = string.Empty;
                Image = string.Empty;
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
                var userProfileId = await GetUserProfileId(linkedInToken);

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


                    regiterUploadModel.registerUploadRequest.owner = Constants.UrnOwner + userProfileId;

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

                        uGCPostRequestModel = UGCShareTextWithImageModel(urnAssestForImageSharing, userProfileId);
                    }
                }
                else
                {
                    uGCPostRequestModel = UGCShareTextModel(userProfileId);
                }


                return uGCPostRequestModel;
            }
            catch (Exception)
            {
                return null;
            }
        }
        private UGCPostRequestModel UGCShareTextModel(string userProfileId)
        {
            var shareTextModel = new UGCPostRequestModel
            {
                author = Constants.UrnOwner + userProfileId,
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
        private UGCPostRequestModel UGCShareTextWithImageModel(string urnAssets, string userProfileId)
        {
            try
            {
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

        private async Task<string> CurrentCheckin(string token)
        {
            try
            {

                await _geoLocatorService.GetLocationAsync();

                var venues = await _fourSquareService.GetVenueList(
                                                Constants.FSVenueSearchURL, token,
                                                _geoLocatorService.Latitude.ToString(), _geoLocatorService.Longitude.ToString(),
                                                DateTime.Now.ToString("yyyyMMdd")
                                                ).ConfigureAwait(false);
                if (venues.IsSuccess)
                {
                    if (venues.ResultData.response.venues.Count > 0)
                        venueID = venues.ResultData.response.venues[0].id;
                    else
                        await _pageDialogService.DisplayAlertAsync("Not Found", "Can't Create Checkin here.", "Ok");
                }
                else
                {
                    await _pageDialogService.DisplayAlertAsync("Message", "Something went wrong, please try again later", "Ok");
                }

                return venueID;
            }
            catch (Exception exception)
            {
                var properties = new Dictionary<string, string>
                    {
                        { "Category", "Venues" },
                        { "Wifi", "On"}
                    };
                Crashes.TrackError(exception, properties);

                return venueID;
            }
            finally
            {
                IsRunning = false;
            }
        }



        #endregion

    }
}
