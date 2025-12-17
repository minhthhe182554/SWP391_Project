using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Services;

namespace SWP391_Project.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet("cities")]
    public async Task<IActionResult> GetCities()
    {
        var cities = await _locationService.GetCitiesAsync();
        return Ok(cities);
    }

    [HttpGet("wards")]
    public async Task<IActionResult> GetWards([FromQuery] string cityCode)
    {
        if (string.IsNullOrWhiteSpace(cityCode)) return BadRequest("cityCode is required");
        var wards = await _locationService.GetWardsByCityCodeAsync(cityCode);
        return Ok(wards);
    }
}
