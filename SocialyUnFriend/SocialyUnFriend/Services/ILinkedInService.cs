using SocialyUnFriend.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialyUnFriend.Services
{
    public interface ILinkedInService
    {
        Task<ApiResponse<object>> GetUserProfile(string url, string accesToken);

        Task<ApiResponse<object>> RegisterUpload(string url, object model, string accessToken);
        Task<ApiResponse<object>> CreatePost(string url, object model, string accessToken);
    }
}
