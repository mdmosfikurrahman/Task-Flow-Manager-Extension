using Task_Flow_Manager_Extension.Dto.Request;
using Task_Flow_Manager_Extension.Dto.Response;
using Task_Flow_Manager_Extension.Exceptions;
using Task_Flow_Manager_Extension.Mappers;
using Task_Flow_Manager_Extension.Models;
using Task_Flow_Manager_Extension.Repositories;
using Task_Flow_Manager_Extension.Validators;

namespace Task_Flow_Manager_Extension.Services.Impl;

public class ClientsServiceImpl(IClientsRepository repository) : IClientsService
{
    public async Task<List<ClientResponse>> GetAll()
    {
        var list = await repository.FindAllAsync();
        if (list.Count == 0)
            throw new NotFoundException("No clients found");

        return list.ToResponseList<Client, ClientResponse>();
    }

    public async Task<ClientResponse> GetById(long id)
    {
        var client = await repository.FindByIdAsync(id);
        if (client == null)
            throw new NotFoundException($"Client not found with id: {id}");

        return client.ToResponse<Client, ClientResponse>();
    }

    public async Task<ClientResponse> Create(ClientRequest request)
    {
        ClientRequestValidator.Validate(request);
        var entity = request.ToEntity<ClientRequest, Client>();
        var saved = await repository.SaveAsync(entity);
        return saved.ToResponse<Client, ClientResponse>();
    }

    public async Task<ClientResponse> Update(long id, ClientRequest request)
    {
        ClientRequestValidator.Validate(request);
        var existing = await repository.FindByIdAsync(id);
        if (existing == null)
            throw new NotFoundException($"Client not found with id: {id}");

        request.MapToExisting(existing);
        var updated = await repository.SaveAsync(existing);
        return updated.ToResponse<Client, ClientResponse>();
    }

    public async Task Delete(long id)
    {
        if (!await repository.ExistsByIdAsync(id))
            throw new NotFoundException($"Client not found with id: {id}");

        await repository.DeleteByIdAsync(id);
    }
}
