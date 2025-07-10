using Task_Flow_Manager_Extension.Dto.Request;
using Task_Flow_Manager_Extension.Dto.Response;
using Task_Flow_Manager_Extension.Services;

namespace Task_Flow_Manager_Extension.Resolvers.Invoices;

[ExtendObjectType(Name = "Mutation")]
public class InvoicesMutation
{
    public async Task<InvoiceResponse> CreateInvoice(InvoiceRequest input, [Service] IInvoicesService service)
        => await service.Create(input);

    public async Task<InvoiceResponse> UpdateInvoice(long id, InvoiceRequest input, [Service] IInvoicesService service)
        => await service.Update(id, input);

    public async Task<bool> DeleteInvoice(long id, [Service] IInvoicesService service)
    {
        await service.Delete(id);
        return true;
    }
}