using System;
using System.Collections.Generic;
using System.Text;

namespace SocialyUnFriend.Common
{
    public static class OAuthConfig
    {
        public static string URL_Linked_In =  string.Format("https://www.linkedin.com/oauth/v2/authorization?response_type=code&client_id={0}" +
                                "&redirect_uri={1}" +
                                "&scope={2}" +
                                "&state={3}",
                                Constants.ClientID,
                                Constants.RedirectURI,
                                Constants.Scopes,
                                Constants.State);
        
        public static string URL_FourSquare = string.Format("https://foursquare.com/oauth2/authenticate?client_id={0}" +

                                "&response_type=code" +
                                "&redirect_uri={1}",
                                Constants.FSClientID,
                                Constants.FSRedirectLink);


        public static string AuthProviderUrl(SocialMediaPlatform providerName)
        {
            string url = "";
            switch (providerName)
            {
                case SocialMediaPlatform.LinkedIn:

                    url = URL_Linked_In;

                    break;
                case SocialMediaPlatform.Facebook:
                    break;
                case SocialMediaPlatform.Twitter:
                    break;
                case SocialMediaPlatform.Instagram:
                    break;
                case SocialMediaPlatform.FourSquare:

                    url = URL_FourSquare;


                    break;
                default:
                    break;
            }

            return url;
        }


       
        public static string AccessTokenUrl(SocialMediaPlatform providerName)
        {
            string url = "";
            switch (providerName)
            {
                case SocialMediaPlatform.LinkedIn:
                    url = "https://www.linkedin.com/uas/oauth2/accessToken";
                    break;
                case SocialMediaPlatform.Facebook:
                    break;
                case SocialMediaPlatform.Twitter:
                    break;
                case SocialMediaPlatform.Instagram:
                    break;
                case SocialMediaPlatform.FourSquare:

                    url = Constants.FSAccessTokenURL;

                    break;
                default:
                    break;
            }

            return url;
        }
    }
}
