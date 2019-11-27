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
using SocialyUnFriend.Common;
using SocialyUnFriend.Model;
using SocialyUnFriend.NetworkController;
using SocialyUnFriend.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IHttpClientController _httpClientController;

        public LinkedInPostPageViewModel(IPageDialogService pageDialogService, ILinkedInService linkedInService, IFourSquareService fourSquareService,
                                         IConnectivity connectivity, IHttpClientController httpClientController)
        {
            _pageDialogService = pageDialogService;
            _linkedInService = linkedInService;
            _fourSquareService = fourSquareService;
            _connectivity = connectivity;
            _httpClientController = httpClientController;


            Pictures = new ObservableCollection<PictureModel>();

            PostCommand = new DelegateCommand(OnPostCommandExecuted);
            OpenCameraCommand = new DelegateCommand(OpenCameraCommandExecuted);

        }

        public DelegateCommand PostCommand { get; }
        public DelegateCommand OpenCameraCommand { get; }


        public string checkInID = "";
        public MediaFile file = null;



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



        private ObservableCollection<PictureModel> _pictures;
        public ObservableCollection<PictureModel> Pictures
        {
            get { return _pictures; }
            set { SetProperty(ref _pictures, value); }
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

                var model = await GetUGCPostRequestModel();

                if (model == null) return;

                var postResponse = await _linkedInService.CreatePost
                    (Constants.LinkedInUGCShareUrl, model, token);

                if (postResponse.IsSuccess)
                {
                    Content = string.Empty;
                    Pictures.Clear();

                    await _pageDialogService.DisplayAlertAsync("Post Created", "You just create a post on linked-in.", "Ok");
                }
                else
                {
                    await _pageDialogService.DisplayAlertAsync("Error", postResponse.ErrorMessage, "Ok");
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
            catch (Exception ex)
            {

            }

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

        private async Task TakeOrPickPictures()
        {
            try
            {
                var result = await _pageDialogService.DisplayActionSheetAsync("Choose", null, null, "Take Selfie", "Pick Photos","Cancel");

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

                    Pictures.Add(new PictureModel { Image = file.Path });
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
                        var images = await CrossMedia.Current.PickPhotosAsync(new PickMediaOptions
                        {
                            CompressionQuality = 75,
                            PhotoSize = PhotoSize.Large,
                            SaveMetaData = true
                        },

                        new MultiPickerOptions
                        {
                             AlbumSelectTitle = "Gallery",
                             BarStyle = MultiPickerBarStyle.BlackOpaque,
                             LoadingTitle = "Loading...",
                             PhotoSelectTitle = "Photo Title",
                             BackButtonTitle = "Back",
                             DoneButtonTitle = "Ok",
                             MaximumImagesCount = 5

                        }
                        );

                        if (images.Count == 0)
                            return;

                        foreach (var item in images)
                        {
                            Pictures.Add(new PictureModel { Image = item.Path });
                        }
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

        private async Task<UGCPostRequestModel> GetUGCPostRequestModel()
        {
            try
            {
                UGCPostRequestModel uGCPostRequestModel = null;

                string urnAssestForImageSharing = "";

                if (Pictures.Count > 0)
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
                                                                         Application.Current.Properties["acces_token"].ToString());

                    if (response.IsSuccess)
                    {
                        var data = JsonConvert.DeserializeObject<RegisterUploadResponse>(response.ResultData.ToString());

                        var uploadUrl = data.value.uploadMechanism.MediaUploadHttpRequest.UpLoadUrl;

                        urnAssestForImageSharing = data.value.asset;

                        //foreach (var item in Pictures)
                        //{
                            await _httpClientController.UploadImage(uploadUrl, ImageHelper.UriPathToBytes(Pictures.Last().Image), 
                                                                    Application.Current.Properties["acces_token"].ToString());
                        //}
                        

                        uGCPostRequestModel = UGCShareTextWithImageModel(urnAssestForImageSharing);
                    }
                }
                else
                {
                    uGCPostRequestModel = UGCShareTextModel();
                }


                return uGCPostRequestModel;
            }
            catch (Exception ex)
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

        private UGCPostRequestModel UGCShareTextWithImageModel(string urnAssets)
        {
            var model = new UGCPostRequestModel
            {
                author = Constants.UrnOwner + Application.Current.Properties["userID"].ToString(),
                lifecycleState = "PUBLISHED",
                specificContent = new SpecificContent
                {
                    ShareContent = new ComLinkedinUgcShareContent
                    {
                        shareCommentary = new ShareCommentary
                        {
                            text = string.IsNullOrEmpty(Content)  ? "  " : Content
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


    }
}
