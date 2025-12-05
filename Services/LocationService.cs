using System.Text.Json;
using SWP391_Project.Dtos;

namespace SWP391_Project.Services;

public class LocationService : ILocationService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private const string BaseUrl = "https://tinhthanhpho.com/api/v1";

    public LocationService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        
        var apiKey = _configuration["API_KEY:LocationKey"];
        if (!string.IsNullOrEmpty(apiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);
        }
    }

    public async Task<List<CityDto>> GetCitiesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/new-provinces");
            response.EnsureSuccessStatusCode();
            
            var jsonString = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<CityDto>>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            return apiResponse?.Data ?? new List<CityDto>();
        }
        catch (Exception)
        {
            return new List<CityDto>();
        }
    }

    public async Task<List<WardDto>> GetWardsByCityCodeAsync(string cityCode)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/new-provinces/{cityCode}/wards");
            response.EnsureSuccessStatusCode();
            
            var jsonString = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiResponse<WardDto>>(jsonString, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            
            return apiResponse?.Data ?? new List<WardDto>();
        }
        catch (Exception)
        {
            return new List<WardDto>();
        }
    }
}
