using Microsoft.AspNetCore.Mvc;
using SWP391_Project.Services;

namespace SWP391_Project.Controllers;

[ApiController]
[Route("api/domains")]
public class DomainController : ControllerBase
{
    private readonly IDomainService _domainService;

    public DomainController(IDomainService domainService)
    {
        _domainService = domainService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? keyword = null)
    {
        var domains = await _domainService.GetDomainsAsync(keyword);
        return Ok(domains);
    }
}
