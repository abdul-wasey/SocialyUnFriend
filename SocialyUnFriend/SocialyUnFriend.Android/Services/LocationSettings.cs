using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SocialyUnFriend.DependencyServcices;

namespace SocialyUnFriend.Droid.Services
{
    public class LocationSettings : ILocationSettings
    {
        public void OpenSettings()
        {
            Context ctx = MainActivity.Instance;
            ctx.StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
        }
    }
}