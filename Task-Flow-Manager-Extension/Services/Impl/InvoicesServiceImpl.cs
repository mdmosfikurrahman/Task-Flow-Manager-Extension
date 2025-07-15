using Microsoft.Extensions.Caching.Distributed;
using Task_Flow_Manager_Extension.Dto.Request;
using Task_Flow_Manager_Extension.Dto.Response;
using Task_Flow_Manager_Extension.Exceptions;
using Task_Flow_Manager_Extension.Mappers;
using Task_Flow_Manager_Extension.Models;
using Task_Flow_Manager_Extension.Repositories;
using Task_Flow_Manager_Extension.Utils;
using Task_Flow_Manager_Extension.Validators;

namespace Task_Flow_Manager_Extension.Services.Impl;

public class InvoicesServiceImpl(
    IInvoicesRepository repository,
    IDistributedCache cache) : IInvoicesService, ICacheWarmable
{
    private readonly EntityCacheManagerUtils<InvoiceResponse> _invoiceCache =
        new(cache, "invoice");

    public string EntityName => "Invoices";

    public async Task<int> RefreshCache()
    {
        var responseList = (await repository.FindAllAsync()).ToResponseList<Invoice, InvoiceResponse>();
        await _invoiceCache.SetAllAsync(responseList);
        foreach (var client in responseList)
            await _invoiceCache.SetByIdAsync(client.Id, client);

        return responseList.Count;
    }

    public async Task<List<InvoiceResponse>> GetAll()
    {
        return await _invoiceCache.GetAllAsync(async () =>
        {
            var list = await repository.FindAllAsync();
            if (list.Count == 0)
                throw new NotFoundException("No invoices found");
            return list.ToResponseList<Invoice, InvoiceResponse>();
        });
    }

    public async Task<InvoiceResponse> GetById(long id)
    {
        return await _invoiceCache.GetByIdAsync(id, async () =>
        {
            var invoice = await repository.FindByIdAsync(id);
            if (invoice == null)
                throw new NotFoundException($"Invoice not found with id: {id}");
            return invoice.ToResponse<Invoice, InvoiceResponse>();
        });
    }

    public async Task<InvoiceResponse> Create(InvoiceRequest request)
    {
        InvoiceRequestValidator.Validate(request);
        var saved = await repository.SaveAsync(request.ToEntity<InvoiceRequest, Invoice>());
        var response = saved.ToResponse<Invoice, InvoiceResponse>();

        await _invoiceCache.SetByIdAsync(response.Id, response);
        await _invoiceCache.SetAllAsync((await repository.FindAllAsync()).ToResponseList<Invoice, InvoiceResponse>());

        return response;
    }

    public async Task<InvoiceResponse> Update(long id, InvoiceRequest request)
    {
        InvoiceRequestValidator.Validate(request);
        var existing = await repository.FindByIdAsync(id)
                       ?? throw new NotFoundException($"Invoice not found with id: {id}");

        request.MapToExisting(existing);
        var updated = await repository.SaveAsync(existing);
        var response = updated.ToResponse<Invoice, InvoiceResponse>();

        await _invoiceCache.SetByIdAsync(id, response);
        await _invoiceCache.SetAllAsync((await repository.FindAllAsync()).ToResponseList<Invoice, InvoiceResponse>());

        return response;
    }

    public async Task Delete(long id)
    {
        if (!await repository.ExistsByIdAsync(id))
            throw new NotFoundException($"Invoice not found with id: {id}");

        await repository.DeleteByIdAsync(id);

        await _invoiceCache.RemoveByIdAsync(id);
        await _invoiceCache.SetAllAsync((await repository.FindAllAsync()).ToResponseList<Invoice, InvoiceResponse>());
    }
}