using Task_Flow_Manager_Extension.Dto.Request;
using Task_Flow_Manager_Extension.Utils;

namespace Task_Flow_Manager_Extension.Validators;

public static class ClientRequestValidator
{
    public static void Validate(ClientRequest request)
    {
        ValidatorUtils.NotEmpty(request.Name, nameof(request.Name));
        ValidatorUtils.MaxLength(request.Name, 100, nameof(request.Name));

        ValidatorUtils.NotEmpty(request.Email, nameof(request.Email));
        ValidatorUtils.MaxLength(request.Email, 100, nameof(request.Email));

        ValidatorUtils.MaxLength(request.Phone, 50, nameof(request.Phone));
        ValidatorUtils.MaxLength(request.CompanyName, 255, nameof(request.CompanyName));
    }
}