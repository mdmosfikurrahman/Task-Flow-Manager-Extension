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

public class ClientsServiceImpl(
    IClientsRepository repository,
    IDistributedCache cache) : IClientsService, ICacheWarmable
{
    private readonly EntityCacheManagerUtils<ClientResponse> _clientCache =
        new(cache, "client");
    
    public string EntityName => "Clients";

    public async Task<int> RefreshCache()
    {
        var responseList = (await repository.FindAllAsync()).ToResponseList<Client, ClientResponse>();
        await _clientCache.SetAllAsync(responseList);
        foreach (var client in responseList)
            await _clientCache.SetByIdAsync(client.Id, client);

        return responseList.Count;
    }

    public async Task<List<ClientResponse>> GetAll()
    {
        return await _clientCache.GetAllAsync(async () =>
        {
            var list = await repository.FindAllAsync();
            if (list.Count == 0)
                throw new NotFoundException("No clients found");
            return list.ToResponseList<Client, ClientResponse>();
        });
    }

    public async Task<ClientResponse> GetById(long id)
    {
        return await _clientCache.GetByIdAsync(id, async () =>
        {
            var client = await repository.FindByIdAsync(id);
            if (client == null)
                throw new NotFoundException($"Client not found with id: {id}");
            return client.ToResponse<Client, ClientResponse>();
        });
    }

    public async Task<ClientResponse> Create(ClientRequest request)
    {
        ClientRequestValidator.Validate(request);
        var saved = await repository.SaveAsync(request.ToEntity<ClientRequest, Client>());
        var response = saved.ToResponse<Client, ClientResponse>();

        await _clientCache.SetByIdAsync(response.Id, response);
        await _clientCache.SetAllAsync((await repository.FindAllAsync()).ToResponseList<Client, ClientResponse>());

        return response;
    }

    public async Task<ClientResponse> Update(long id, ClientRequest request)
    {
        ClientRequestValidator.Validate(request);
        var existing = await repository.FindByIdAsync(id)
                       ?? throw new NotFoundException($"Client not found with id: {id}");

        request.MapToExisting(existing);
        var updated = await repository.SaveAsync(existing);
        var response = updated.ToResponse<Client, ClientResponse>();

        await _clientCache.SetByIdAsync(id, response);
        await _clientCache.SetAllAsync((await repository.FindAllAsync()).ToResponseList<Client, ClientResponse>());

        return response;
    }

    public async Task Delete(long id)
    {
        if (!await repository.ExistsByIdAsync(id))
            throw new NotFoundException($"Client not found with id: {id}");

        await repository.DeleteByIdAsync(id);

        await _clientCache.RemoveByIdAsync(id);
        await _clientCache.SetAllAsync((await repository.FindAllAsync()).ToResponseList<Client, ClientResponse>());
    }
}