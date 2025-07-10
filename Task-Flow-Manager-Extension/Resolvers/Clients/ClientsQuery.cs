using Task_Flow_Manager_Extension.Dto.Response;
using Task_Flow_Manager_Extension.Services;

namespace Task_Flow_Manager_Extension.Resolvers.Clients;

[ExtendObjectType(Name = "Query")]
public class ClientsQuery
{
    public async Task<List<ClientResponse>> GetClients([Service] IClientsService service)
        => await service.GetAll();

    public async Task<ClientResponse> GetClientById(long id, [Service] IClientsService service)
        => await service.GetById(id);
}