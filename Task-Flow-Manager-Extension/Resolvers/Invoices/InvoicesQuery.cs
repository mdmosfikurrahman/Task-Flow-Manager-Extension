using Task_Flow_Manager_Extension.Dto.Response;
using Task_Flow_Manager_Extension.Services;

namespace Task_Flow_Manager_Extension.Resolvers.Invoices;

[ExtendObjectType(Name = "Query")]
public class InvoicesQuery
{
    public async Task<List<InvoiceResponse>> GetInvoices([Service] IInvoicesService service)
        => await service.GetAll();

    public async Task<InvoiceResponse> GetInvoiceById(long id, [Service] IInvoicesService service)
        => await service.GetById(id);
}