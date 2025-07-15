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
    public async Task<List<ClientResponse>> GetAll()
    {
        return await cache.GetOrSetAsync("clients_all", async () =>
        {
            var list = await repository.FindAllAsync();
            if (list.Count == 0)
                throw new NotFoundException("No clients found");
            return list.ToResponseList<Client, ClientResponse>();
        });
    }

    public async Task<ClientResponse> GetById(long id)
    {
        string key = $"client_{id}";
        return await cache.GetOrSetAsync(key, async () =>
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
        var entity = request.ToEntity<ClientRequest, Client>();
        var saved = await repository.SaveAsync(entity);
        var response = saved.ToResponse<Client, ClientResponse>();

        await cache.SetAsync($"client_{response.Id}", response);
        var all = await repository.FindAllAsync();
        await cache.SetAsync("clients_all", all.ToResponseList<Client, ClientResponse>());

        return response;
    }

    public async Task<ClientResponse> Update(long id, ClientRequest request)
    {
        ClientRequestValidator.Validate(request);
        var existing = await repository.FindByIdAsync(id);
        if (existing == null)
            throw new NotFoundException($"Client not found with id: {id}");

        request.MapToExisting(existing);
        var updated = await repository.SaveAsync(existing);
        var response = updated.ToResponse<Client, ClientResponse>();

        await cache.SetAsync($"client_{id}", response);
        var all = await repository.FindAllAsync();
        await cache.SetAsync("clients_all", all.ToResponseList<Client, ClientResponse>());

        return response;
    }

    public async Task Delete(long id)
    {
        if (!await repository.ExistsByIdAsync(id))
            throw new NotFoundException($"Client not found with id: {id}");

        await repository.DeleteByIdAsync(id);

        await cache.RemoveAsync($"client_{id}");
        var all = await repository.FindAllAsync();
        await cache.SetAsync("clients_all", all.ToResponseList<Client, ClientResponse>());
    }

    public string EntityName => "Clients";
    public async Task<int> RefreshCache()
    {
        var list = await repository.FindAllAsync();
        var responseList = list.ToResponseList<Client, ClientResponse>();

        await cache.SetAsync("clients_all", responseList);
        foreach (var client in responseList)
        {
            await cache.SetAsync($"client_{client.Id}", client);
        }

        return responseList.Count;
    }
}
