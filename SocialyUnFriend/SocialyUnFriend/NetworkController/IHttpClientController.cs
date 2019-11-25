using SocialyUnFriend.Common;
using SocialyUnFriend.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialyUnFriend.NetworkController
{
    public interface IHttpClientController
    {
        Task<ApiResponse<OAuthToken>> GetOAuthTokenAsync(string url,string OAuthCode, string clientId, string clientSecret, string redirectUrl);
        Task<ApiResponse<object>> PostAsync(string url, object model, string accesToken);
        Task<ApiResponse<object>> GetAsync<T>(string url, string token);

        Task<ApiResponse<FourSquareProfile>> GetFourSquareUserProfile(string url, string token, string vDate);
        Task<ApiResponse<Venues>> GetVenueList(string url, string accesToken, string lat, string lng, string vDate);
        Task<ApiResponse<CheckInResponse>> CreateCheckInByVenueId(string url, string venueId, string vDate, string accessToken);
        Task<ApiResponse<object>> AddCheckinPost(string url, string checkInId, string text, string vDate, string accessToken);

    }
}
