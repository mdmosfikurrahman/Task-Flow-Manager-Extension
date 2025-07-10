using Task_Flow_Manager_Extension.Dto.Request;
using Task_Flow_Manager_Extension.Dto.Response;

namespace Task_Flow_Manager_Extension.Services;

public interface IInvoicesService
{
    Task<List<InvoiceResponse>> GetAll();
    Task<InvoiceResponse> GetById(long id);
    Task<InvoiceResponse> Create(InvoiceRequest request);
    Task<InvoiceResponse> Update(long id, InvoiceRequest request);
    Task Delete(long id);
}