using Grpc.Core;
using GrpcContracts;
using Task_Flow_Manager_Extension.Services;

namespace Task_Flow_Manager_Extension.Grpc;

public class ClientGrpcService(IClientsService service) : ClientService.ClientServiceBase
{
    public override async Task<ClientResponse> GetClientById(ClientRequest request, ServerCallContext context)
    {
        var client = await service.GetById(request.Id);

        return new ClientResponse
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            Phone = client.Phone ?? "",
            CompanyName = client.CompanyName ?? ""
        };
    }
}