using Plugin.Permissions;
using Prism.Services;
using SocialyUnFriend.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SocialyUnFriend.Services
{
    public class GeoLocatorService : IGeoLocatorService
    {
        private readonly IPageDialogService _pageDialogService;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public GeoLocatorService(IPageDialogService pageDialogService)
        {
            _pageDialogService = pageDialogService;
        }

        public async Task GetLocationAsync()
        {

            try
            {
                var status = await Utils.CheckPermissions<LocationPermission>();

                if (status == Plugin.Permissions.Abstractions.PermissionStatus.Granted)
                {
                    var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(15));

                    var location = await Geolocation.GetLocationAsync(request);

                    if (location != null)
                    {
                        Latitude = location.Latitude;
                        Longitude = location.Longitude;
                    }
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                await _pageDialogService.DisplayAlertAsync("Not Supported", "This feature is not supported on this device", "Ok");
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception

                var isProceed = await _pageDialogService.DisplayAlertAsync("Attention", "Please enable the device GPS", "Proceed", "Maybe Later");

                if (isProceed)
                    CrossPermissions.Current.OpenAppSettings();


            }
            catch (PermissionException pEx)
            {
                // Handle permission exception

                await _pageDialogService.DisplayAlertAsync("Permission Required", "Please enable the GPS permission for this App.", "Ok");

            }
            catch (Exception ex)
            {
                // Unable to get location
                await _pageDialogService.DisplayAlertAsync("Error", "Unable to get location, please try later", "Ok");

            }
        }
    }
}
