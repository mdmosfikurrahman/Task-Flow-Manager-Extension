using Task_Flow_Manager_Extension.Dto.Request;
using Task_Flow_Manager_Extension.Dto.Response;
using Task_Flow_Manager_Extension.Services;

namespace Task_Flow_Manager_Extension.Resolvers.Clients;

[ExtendObjectType(Name = "Mutation")]
public class ClientsMutation
{
    public async Task<ClientResponse> CreateClient(ClientRequest input, [Service] IClientsService service)
        => await service.Create(input);

    public async Task<ClientResponse> UpdateClient(long id, ClientRequest input, [Service] IClientsService service)
        => await service.Update(id, input);

    public async Task<bool> DeleteClient(long id, [Service] IClientsService service)
    {
        await service.Delete(id);
        return true;
    }
}