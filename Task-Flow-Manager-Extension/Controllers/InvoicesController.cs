using Microsoft.AspNetCore.Mvc;
using Task_Flow_Manager_Extension.Dto.Request;
using Task_Flow_Manager_Extension.Dto.Response;
using Task_Flow_Manager_Extension.Infrastructure.Dto;
using Task_Flow_Manager_Extension.Services;

namespace Task_Flow_Manager_Extension.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/invoices")]
public class InvoicesController(IInvoicesService service) : ControllerBase
{
    [HttpGet]
    public async Task<RestResponse<List<InvoiceResponse>>> GetAll()
        => RestResponse<List<InvoiceResponse>>.Success(200, "Fetched", await service.GetAll());

    [HttpGet("{id}")]
    public async Task<RestResponse<InvoiceResponse>> GetById(long id)
        => RestResponse<InvoiceResponse>.Success(200, "Fetched", await service.GetById(id));

    [HttpPost]
    public async Task<RestResponse<InvoiceResponse>> Create([FromBody] InvoiceRequest request)
        => RestResponse<InvoiceResponse>.Success(201, "Created", await service.Create(request));

    [HttpPut("{id}")]
    public async Task<RestResponse<InvoiceResponse>> Update(long id, [FromBody] InvoiceRequest request)
        => RestResponse<InvoiceResponse>.Success(200, "Updated", await service.Update(id, request));

    [HttpDelete("{id}")]
    public async Task<RestResponse<string>> Delete(long id)
    {
        await service.Delete(id);
        return RestResponse<string>.Success(200, "Deleted", "Success");
    }
}