using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SocialyUnFriend.Model;
using SocialyUnFriend.NetworkController;

namespace SocialyUnFriend.Services
{
    public class FourSquareService : IFourSquareService
    {
        private readonly IHttpClientController _httpClientController;

        public FourSquareService(IHttpClientController httpClientController)
        {
            _httpClientController = httpClientController;
        }

      

        public async Task<ApiResponse<FourSquareProfile>> GetUserProfile(string url, string accesToken, string versionDate)
        {
            return await _httpClientController.GetFourSquareUserProfile(url, accesToken,versionDate);
        }

        public async Task<ApiResponse<Venues>> GetVenueList(string url, string accesToken, string lat, string lng, string vDate)
        {
           return await _httpClientController.GetVenueList(url, accesToken, lat, lng, vDate);
        }

        public Task<ApiResponse<CheckInResponse>> CreateCheckIn(string url, string accesToken, string venueId, string vDate)
        {
            return _httpClientController.CreateCheckInByVenueId(url, venueId, vDate, accesToken);
        }

        public Task<ApiResponse<object>> AddCheckinPost(string url, string accessToken, string checkInId, string text, string vDate)
        {
            return _httpClientController.AddCheckinPost(url, checkInId, text, vDate, accessToken);
        }
    }
}
