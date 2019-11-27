using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using DryIoc;
using System.Text;
using SocialyUnFriend.Model;
using SocialyUnFriend.Common;
using Xamarin.Forms;
using System.IO;
using System.Globalization;

namespace SocialyUnFriend.NetworkController
{
    public class HttpClientController : IHttpClientController
    {
        private readonly HttpClient _httpClient;

        public HttpClientController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<object>> UploadImage(string uploadUrl, byte[] image, string accessToken)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                ByteArrayContent byteArrayContent = new ByteArrayContent(image);

                byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
              
                var response = await _httpClient.PutAsync(uploadUrl, byteArrayContent);


                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<object>
                    {
                        IsSuccess = false,
                        ErrorMessage = result,
                    };
                }


                return new ApiResponse<object>
                {
                    IsSuccess = true,
                    ResultData = result
                };
           
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ApiResponse<OAuthToken>> GetOAuthTokenAsync(string url, string code, string clientId, string clientSecret, string redirectUrl)
        {
            try
            {
                var keyValuePairs = new Dictionary<string, string>
                {
                    { "client_id", clientId},
                    { "client_secret",clientSecret},
                    {"grant_type","authorization_code"},
                    {"redirect_uri", redirectUrl},
                    {"code",code}
                };

                var content = new FormUrlEncodedContent(keyValuePairs);

                var response = await _httpClient.PostAsync(url, content);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<OAuthToken>
                    {
                        IsSuccess = false,
                        ErrorMessage = result,
                    };
                }

                var token = JsonConvert.DeserializeObject<OAuthToken>(result);

                return new ApiResponse<OAuthToken>
                {
                    IsSuccess = true,
                    ResultData = token
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<OAuthToken>
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }

        }


        public async Task<ApiResponse<object>> PostAsync(string url, object model, string accesToken)
        {
            try
            {

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesToken);
                //_httpClient.DefaultRequestHeaders.Add("X-Restli-Protocol-Version", "2.0.0");

                var json = JsonConvert.SerializeObject(model);

                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(url, stringContent);


                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<object>
                    {
                        IsSuccess = false,
                        ErrorMessage = result,
                    };
                }


                return new ApiResponse<object>
                {
                    IsSuccess = true,
                    ResultData = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ApiResponse<object>> GetAsync<T>(string url, string token)
        {
            try
            {

                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);


                var response = await _httpClient.GetAsync(url);


                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<object>
                    {
                        IsSuccess = false,
                        ErrorMessage = result,
                    };
                }

                var responseResult = JsonConvert.DeserializeObject<T>(result);

                return new ApiResponse<object>
                {
                    IsSuccess = true,
                    ResultData = responseResult
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };

                
            }
        }

        #region FourSquare Api Calls

        public async Task<ApiResponse<FourSquareProfile>> GetFourSquareUserProfile(string url, string token, string vDate)
        {
            try
            {
                string urlEncoded = url;

                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                urlEncoded = string.Format(url + "?oauth_token={0}" + "&v={1}", token, vDate);

                var response = await _httpClient.GetAsync(urlEncoded);


                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<FourSquareProfile>
                    {
                        IsSuccess = false,
                        ErrorMessage = result,
                    };
                }

                var responseResult = JsonConvert.DeserializeObject<FourSquareProfile>(result);

                return new ApiResponse<FourSquareProfile>
                {
                    IsSuccess = true,
                    ResultData = responseResult
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<FourSquareProfile>
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }
        public async Task<ApiResponse<Venues>> GetVenueList(string url, string accesToken, string lat, string lng, string vDate)
        {
            try
            {
                string urlEncoded = url;

                string latlng = lat + "," + lng;
                
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                urlEncoded = string.Format(url + "?oauth_token={0}" + "&ll={1}" + "&v={2}", accesToken, latlng,vDate);

                var response = await _httpClient.GetAsync(urlEncoded);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<Venues>
                    {
                        IsSuccess = false,
                        ErrorMessage = result,
                    };
                }

                var responseResult = JsonConvert.DeserializeObject<Venues>(result);

                return new ApiResponse<Venues>
                {
                    IsSuccess = true,
                    ResultData = responseResult
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<Venues>
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }
        public async Task<ApiResponse<CheckInResponse>> CreateCheckInByVenueId(string url, string venueId, string vDate, string accessToken)
        {
            try
            {
                var queryParameters = new Dictionary<string, string>
                {
                    { "oauth_token", accessToken},
                    { "venueId",venueId},
                    {"v", vDate}
                };

                var content = new FormUrlEncodedContent(queryParameters);

                var response = await _httpClient.PostAsync(url, content);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<CheckInResponse>
                    {
                        IsSuccess = false,
                        ErrorMessage = result,
                    };
                }

                var data = JsonConvert.DeserializeObject<CheckInResponse>(result);
                return new ApiResponse<CheckInResponse>
                {
                    IsSuccess = true,
                    ResultData = data
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<CheckInResponse>
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<ApiResponse<object>> AddCheckinPost(string url, string checkInId, string text, string vDate, string accessToken)
        {
            try
            {
                string completeUrl = string.Format(Constants.FSCheckInPostURL + "/{0}" + "/addpost", checkInId);

                var queryParameters = new Dictionary<string, string>
                {
                    { "oauth_token", accessToken},
                    { "text", text},
                    { "v", vDate}
                };

                var content = new FormUrlEncodedContent(queryParameters);

                var response = await _httpClient.PostAsync(completeUrl, content);

                var result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return new ApiResponse<object>
                    {
                        IsSuccess = false,
                        ErrorMessage = result,
                    };
                }

              
                return new ApiResponse<object>
                {
                    IsSuccess = true,
                    ResultData = result
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
            }
        }


        #endregion
    }
}
