using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialyUnFriend.Model
{
    public class OAuthToken
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public string ExpireDate { get; set; }

        [JsonProperty("refresh_token")]
        public long RefreshToken { get; set; }

        [JsonProperty("refresh_token_expires_in")]
        public string RTExpireDate { get; set; }
        
    }
}
