using SWP391_Project.Dtos;

namespace SWP391_Project.Services;

public interface ILocationService
{
    Task<List<CityDto>> GetCitiesAsync();
    Task<List<WardDto>> GetWardsByCityCodeAsync(string cityCode);
}
