using Microsoft.AspNetCore.Mvc;
using Task_Flow_Manager_Extension.Infrastructure.Dto;
using Task_Flow_Manager_Extension.Services;

namespace Task_Flow_Manager_Extension.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/cache")]
public class CacheController(IEnumerable<ICacheWarmable> cacheWarmables) : ControllerBase
{
    [HttpPost("refresh")]
    public async Task<RestResponse<Dictionary<string, int>>> RefreshAll()
    {
        var result = new Dictionary<string, int>();

        foreach (var service in cacheWarmables)
        {
            var count = await service.RefreshCache();
            result[service.EntityName] = count;
        }

        return RestResponse<Dictionary<string, int>>.Success(
            200,
            "Cache refreshed for all entities",
            result
        );
    }
}