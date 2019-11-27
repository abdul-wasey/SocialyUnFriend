using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SocialyUnFriend.Model;
using SocialyUnFriend.NetworkController;

namespace SocialyUnFriend.Services
{
    public class LinkedInService : ILinkedInService
    {
        private readonly IHttpClientController _httpClientController;

        public LinkedInService(IHttpClientController httpClientController)
        {
            _httpClientController = httpClientController;
        }

        public async Task<ApiResponse<object>> GetUserProfile(string url, string accesToken)
        {
            return await _httpClientController.GetAsync<LinkedInProfile>(url, accesToken);
        }

        public async Task<ApiResponse<object>> CreatePost(string url, object model, string accessToken)
        {
            return await _httpClientController.PostAsync(url, model, accessToken);
        }

        public async Task<ApiResponse<object>> RegisterUpload(string url, object model, string accessToken)
        {
            return await _httpClientController.PostAsync(url, model, accessToken);
        }
    }
}
