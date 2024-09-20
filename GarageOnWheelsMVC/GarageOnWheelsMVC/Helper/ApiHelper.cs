using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;

namespace GarageOnWheelsMVC.Helper
{
    public class ApiHelper
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUrl;
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ApiHelper(HttpClient httpClient, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
            _baseUrl = configuration["AppSettings:BaseUrl"];
        }

        // Set Authorization Token
        public void SetAuthorize()
        {
            var token = _httpContextAccessor.HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Get request
        public async Task<T?> GetAsync<T>(string endpoint) where T : class
        {
            SetAuthorize();
            var response = await _httpClient.GetAsync($"{_baseUrl}{endpoint}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
            }
            return null;
        }

        // Send Post or Put request with JSON data
        public async Task<HttpResponseMessage> SendJsonAsync<T>(string endpoint, T model, HttpMethod method)
        {
            SetAuthorize();
            var jsonModel = JsonSerializer.Serialize(model, JsonOptions);
            var content = new StringContent(jsonModel, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(method, $"{_baseUrl}{endpoint}") { Content = content };
            return await _httpClient.SendAsync(request);
        }

        // Send Post request
        public async Task<HttpResponseMessage> SendPostRequest<T>(string endpoint, T model)
        {
            return await SendJsonAsync(endpoint, model, HttpMethod.Post);
        }

        // Check if a specific query result exists
        public async Task<bool> CheckIfExists(string query)
        {
            SetAuthorize();
            var response = await _httpClient.GetAsync($"{_baseUrl}{query}");
            return JsonSerializer.Deserialize<bool>(await response.Content.ReadAsStringAsync());
        }

        // Send OTP to user's email
        public async Task SendOtp(string email)
        {
            SetAuthorize();
            var otpsendResponse = await _httpClient.GetAsync($"{_baseUrl}auth/send-otp/{email}");
            if (!otpsendResponse.IsSuccessStatusCode)
            {
                throw new Exception("Failed to send OTP. Please try again.");
            }
        }
     
    }

}
