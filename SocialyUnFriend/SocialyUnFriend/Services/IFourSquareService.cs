using SocialyUnFriend.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialyUnFriend.Services
{
    public interface IFourSquareService
    {
        Task<ApiResponse<FourSquareProfile>> GetUserProfile(string url, string accesToken,string vDate);
        Task<ApiResponse<Venues>> GetVenueList(string url, string accesToken,string lat,string lng,string vDate);
        Task<ApiResponse<CheckInResponse>> CreateCheckIn(string url, string accesToken,string venueId,string vDate);

        Task<ApiResponse<object>> AddCheckinPost(string url, string accessToken, string checkInId,string text, string vDate);
    }
}
