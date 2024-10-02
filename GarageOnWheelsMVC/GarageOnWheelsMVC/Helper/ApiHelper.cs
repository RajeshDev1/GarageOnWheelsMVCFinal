using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace GarageOnWheelsMVC.Helper
{
    public class ApiHelper
    {
        private readonly HttpClient _httpClient;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string baseUrl;
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ApiHelper(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            baseUrl = configuration["AppSettings:BaseUrl"];
        }

        // Set Authorization Token
        public void SetAuthorize(HttpContext httpContext)
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "");
            }
        }

        // Get request
        public async Task<T?> GetAsync<T>(string endpoint, HttpContext httpContext) where T : class
        {
            SetAuthorize(httpContext);
            var response = await _httpClient.GetAsync($"{baseUrl}{endpoint}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
            }
            return null;
        }

        // Send Post or Put request with JSON data
        public async Task<HttpResponseMessage> SendJsonAsync<T>(string endpoint, T model, HttpMethod method, HttpContext httpContext)
        {
            SetAuthorize(httpContext);
            var jsonModel = JsonSerializer.Serialize(model, JsonOptions);
            var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(method, $"{baseUrl}{endpoint}") { Content = content };
            return await _httpClient.SendAsync(request);
        }

        // Send Post request
        public async Task<HttpResponseMessage> SendPostRequest<T>(string endpoint, T model, HttpContext httpContext)
        {
            return await SendJsonAsync(endpoint, model, HttpMethod.Post, httpContext);
        }

        // Check if a specific query result exists
        public async Task<bool> CheckIfExists(string query,HttpContext httpContext)
        {
            SetAuthorize(httpContext);
            var response = await _httpClient.GetAsync($"{baseUrl}{query}");
            return JsonSerializer.Deserialize<bool>(await response.Content.ReadAsStringAsync());
        }

        // Send OTP to user's email
        public async Task SendOtp(string email, HttpContext httpContext)
        {
            SetAuthorize(httpContext);
            var otpsendResponse = await _httpClient.GetAsync($"{baseUrl}auth/send-otp/{email}");
            if (!otpsendResponse.IsSuccessStatusCode)
            {
                throw new Exception("Failed to send OTP. Please try again.");
            }
        }

        public async Task<bool> GetBoolAsync(string endpoint,HttpContext httpContext)
        {
            SetAuthorize(httpContext);
            var response = await _httpClient.GetAsync($"{baseUrl}{endpoint}");
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<bool>(result, JsonOptions);
            }
            return false; 
        }

        public async Task<HttpResponseMessage> DeleteAsync(string endpoint, HttpContext httpContext)
        {
            SetAuthorize(httpContext);
            try
            {
                var fullUrl = $"{baseUrl}{endpoint}";  
                var response = await _httpClient.DeleteAsync(fullUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Delete request failed. Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during DELETE request: {ex.Message}");
                throw;
            }
        }

        public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, HttpContext httpContext)
        {
            SetAuthorize(httpContext);
            return await _httpClient.SendAsync(request);
        }

    }

}
