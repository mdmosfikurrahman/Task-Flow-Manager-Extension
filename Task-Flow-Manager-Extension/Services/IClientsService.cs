using Task_Flow_Manager_Extension.Dto.Request;
using Task_Flow_Manager_Extension.Dto.Response;

namespace Task_Flow_Manager_Extension.Services;

public interface IClientsService
{
    Task<List<ClientResponse>> GetAll();
    Task<ClientResponse> GetById(long id);
    Task<ClientResponse> Create(ClientRequest request);
    Task<ClientResponse> Update(long id, ClientRequest request);
    Task Delete(long id);
}