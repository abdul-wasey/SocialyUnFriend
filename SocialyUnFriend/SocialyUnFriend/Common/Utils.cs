using Microsoft.AppCenter.Crashes;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SocialyUnFriend.Common
{
    public static class Utils
    {
        public static async Task<PermissionStatus> CheckPermissions<T>(Permission permission) where T : BasePermission, new()
        {
            try
            {
                var permissionStatus = await CrossPermissions.Current.CheckPermissionStatusAsync<T>();
                bool request = false;

                if (permissionStatus == PermissionStatus.Denied)
                {
                    if (Device.RuntimePlatform == Device.iOS)
                    {

                        var title = $"{permission} Permission";
                        var question = $"To use this App {permission} permission is required.";
                        var positive = "Settings";
                        var negative = "Maybe Later";
                        var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
                        if (task == null)
                            return permissionStatus;

                        var result = await task;
                        if (result)
                        {
                            CrossPermissions.Current.OpenAppSettings();
                        }

                        return permissionStatus;
                    }

                    request = true;

                }

                if (request || permissionStatus != PermissionStatus.Granted)
                {
                    var shouldShowRequestDialog = await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(permission);

                    if (shouldShowRequestDialog)
                    {
                        var newStatus = await CrossPermissions.Current.RequestPermissionAsync<T>();

                        permissionStatus = newStatus;
                    }
                    else
                    {
                        if (permissionStatus != PermissionStatus.Granted)
                        {
                            var title = $"{permission} Permission";
                            var question = $"To use this App the {permission} permission is required.";
                            var positive = "Settings";
                            var negative = "Maybe Later";
                            var task = Application.Current?.MainPage?.DisplayAlert(title, question, positive, negative);
                            if (task == null)
                                return permissionStatus;

                            var result = await task;
                            if (result)
                            {
                                CrossPermissions.Current.OpenAppSettings();
                            }

                        }

                    }
                }

                return permissionStatus;
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex);
                return PermissionStatus.Unknown;
            }
        }
    }
}
