﻿using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Essentials;

namespace SocialyUnFriend.Common
{
    public static class Constants
    {
        public const string AppName = "SocialyUnFriend";


        public const string UrnOwner = "urn:li:person:";

        #region LinkedIn Ceredentials

        public const string LinkedInPermittedServicesUrl = "https://www.linkedin.com/psettings/permitted-services";

        public const string LinkedInProfileUrl = "https://api.linkedin.com/v2/me?projection=(id,localizedFirstName,localizedLastName,profilePicture(displayImage~:playableStreams))";

        public const string LinkedInPostShareUrl = "https://api.linkedin.com/v2/shares";

        public const string LinkedInUGCShareUrl = "https://api.linkedin.com/v2/ugcPosts";

        public const string LinkedInRegisterUploadUrl = "https://api.linkedin.com/v2/assets?action=registerUpload";   // Belongs to LinkedIn Assests Api,

        public const string RedirectURI = "https://www.linkedin.com/oauth-success";

        public const string AccessTokenURL = "https://www.linkedin.com/oauth/v2/accessToken";
        public const string AuthorizeURL = "https://www.linkedin.com/oauth/v2/authorization";

        public const string ClientID = "819wcyjuz4i74h";
        public const string ClientSecret = "31KV7dI1j6fsPkcz";
        public const string Scopes = "r_emailaddress r_liteprofile w_member_social";
        public const string State = "ARandomStringToPreventCSRFattacks";

        #endregion

        #region FourSquare Ceredentials

        public const string FSClientID = "ZN0QUUTGYP0LRAN53HLSRWSD45DCRLM1LIH0HQEF2T05WBIX";
        public const string FSClientSecret = "ZGCEFLSASFDVT3JMWFL40TZ5VMAEAGBYNNRCJKXGSZMMYPSC";
        public const string FSRedirectLink = "https://developer.foursquare.com/docs/announcements";

        public const string FSAuthorizeURL = "https://foursquare.com/oauth2/authenticate";


        public const string FSConnectedAppsURL = "https://foursquare.com/settings/connections";

        public const string FSAccessTokenURL = "https://foursquare.com/oauth2/access_token";

        public const string FSUserProfileURL = "https://api.foursquare.com/v2/users/self";
        public const string FSVenueSearchURL = "https://api.foursquare.com/v2/venues/search";
        public const string FSCreateCheckInURL = "https://api.foursquare.com/v2/checkins/add";

        public const string FSCheckInPostURL = "https://api.foursquare.com/v2/checkins/";

        public const string FSAddPhotoURL = "https://api.foursquare.com/v2/photos/add";

        #endregion


        /// <summary>
        /// Both iOS and Android have a specific/unique file directory for each app, and this is where we'll store the .db3 database file. 
        /// We can easily locate it using Xamarin.Essentials.FileSystem.
        /// </summary>
        public static string dbPath = Path.Combine(FileSystem.AppDataDirectory, $"{nameof(SocialyUnFriend)}.db3");


        #region AppCenter Credentials

        public const string AppSecretAndroid = "4e9c405a-c9e0-4f86-9405-e5efd9cc4ca9";
        public const string AppSecretiOS = "82eca861-940d-414d-89f2-3276598e7a88";
        public const string AppSecretUWP = "2d7144cf-b763-4a55-954e-07a063d99183";

        #endregion



        #region Persistance Data's Keys

        public const string IsWelcomePageVisible = "WelcomePageVisible";
        public const string IsAccessTokenAvailable = "IsAccessTokenAvailable";
        public const string AccessTokenLinkedin = "AccessTokenLinkedin";
        public const string AccessTokenLinkedinExpiryDate = "AccessTokenLinkedinExpiryDate";
        public const string LinkedInUserId = "LinkedInUserId";


        public const string AccessTokenFourSquare = "AccessTokenFourSquare";
        public const string IsLinkedInConnected = "IsLinkedInConnected";
        public const string IsFourSquareConnected = "IsFourSquareConnected";



        #endregion

    }
}
