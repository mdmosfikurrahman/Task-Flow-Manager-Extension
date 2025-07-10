using Microsoft.AspNetCore.Mvc;
using Task_Flow_Manager_Extension.Dto.Request;
using Task_Flow_Manager_Extension.Dto.Response;
using Task_Flow_Manager_Extension.Infrastructure.Dto;
using Task_Flow_Manager_Extension.Services;

namespace Task_Flow_Manager_Extension.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/clients")]
public class ClientsController(IClientsService service) : ControllerBase
{
    [HttpGet]
    public async Task<RestResponse<List<ClientResponse>>> GetAll()
        => RestResponse<List<ClientResponse>>.Success(200, "Fetched", await service.GetAll());

    [HttpGet("{id}")]
    public async Task<RestResponse<ClientResponse>> GetById(long id)
        => RestResponse<ClientResponse>.Success(200, "Fetched", await service.GetById(id));

    [HttpPost]
    public async Task<RestResponse<ClientResponse>> Create([FromBody] ClientRequest request)
        => RestResponse<ClientResponse>.Success(201, "Created", await service.Create(request));

    [HttpPut("{id}")]
    public async Task<RestResponse<ClientResponse>> Update(long id, [FromBody] ClientRequest request)
        => RestResponse<ClientResponse>.Success(200, "Updated", await service.Update(id, request));

    [HttpDelete("{id}")]
    public async Task<RestResponse<string>> Delete(long id)
    {
        await service.Delete(id);
        return RestResponse<string>.Success(200, "Deleted", "Success");
    }
}