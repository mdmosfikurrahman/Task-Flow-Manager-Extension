using Task_Flow_Manager_Extension.Dto.Request;
using Task_Flow_Manager_Extension.Dto.Response;
using Task_Flow_Manager_Extension.Exceptions;
using Task_Flow_Manager_Extension.Mappers;
using Task_Flow_Manager_Extension.Models;
using Task_Flow_Manager_Extension.Repositories;
using Task_Flow_Manager_Extension.Validators;

namespace Task_Flow_Manager_Extension.Services.Impl;

public class InvoicesServiceImpl(IInvoicesRepository repository) : IInvoicesService
{
    public async Task<List<InvoiceResponse>> GetAll()
    {
        var list = await repository.FindAllAsync();
        if (list.Count == 0)
            throw new NotFoundException("No invoices found");

        return list.ToResponseList<Invoices, InvoiceResponse>();
    }

    public async Task<InvoiceResponse> GetById(long id)
    {
        var invoice = await repository.FindByIdAsync(id);
        if (invoice == null)
            throw new NotFoundException($"Invoice not found with id: {id}");

        return invoice.ToResponse<Invoices, InvoiceResponse>();
    }

    public async Task<InvoiceResponse> Create(InvoiceRequest request)
    {
        InvoiceRequestValidator.Validate(request);
        var entity = request.ToEntity<InvoiceRequest, Invoices>();
        var saved = await repository.SaveAsync(entity);
        return saved.ToResponse<Invoices, InvoiceResponse>();
    }

    public async Task<InvoiceResponse> Update(long id, InvoiceRequest request)
    {
        InvoiceRequestValidator.Validate(request);
        var existing = await repository.FindByIdAsync(id);
        if (existing == null)
            throw new NotFoundException($"Invoice not found with id: {id}");

        request.MapToExisting(existing);
        var updated = await repository.SaveAsync(existing);
        return updated.ToResponse<Invoices, InvoiceResponse>();
    }

    public async Task Delete(long id)
    {
        if (!await repository.ExistsByIdAsync(id))
            throw new NotFoundException($"Invoice not found with id: {id}");

        await repository.DeleteByIdAsync(id);
    }
}
