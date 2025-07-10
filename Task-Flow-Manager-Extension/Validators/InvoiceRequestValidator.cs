using Task_Flow_Manager_Extension.Dto.Request;
using Task_Flow_Manager_Extension.Utils;

namespace Task_Flow_Manager_Extension.Validators;

public static class InvoiceRequestValidator
{
    public static void Validate(InvoiceRequest request)
    {
        ValidatorUtils.MinValue(request.ClientId, 1, nameof(request.ClientId));
        ValidatorUtils.NotEmpty(request.InvoiceNumber, nameof(request.InvoiceNumber));
        ValidatorUtils.MaxLength(request.InvoiceNumber, 50, nameof(request.InvoiceNumber));
        ValidatorUtils.MinValue(request.Amount, 0.01m, nameof(request.Amount));
    }
}