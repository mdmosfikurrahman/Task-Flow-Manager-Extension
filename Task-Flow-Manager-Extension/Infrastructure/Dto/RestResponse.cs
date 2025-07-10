using System.Text.Json.Serialization;
using Task_Flow_Manager_Extension.Utils;

namespace Task_Flow_Manager_Extension.Infrastructure.Dto;

public class RestResponse<T>
{
    [JsonPropertyName("timestamp")] public string Timestamp { get; set; }

    [JsonPropertyName("status")] public int Status { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; }

    [JsonPropertyName("data")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public T? Data { get; set; }

    [JsonPropertyName("errors")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public List<ErrorDetails>? Errors { get; set; }

    private RestResponse(int status, string message, T? data = default, List<ErrorDetails>? errors = null)
    {
        Timestamp = TimeUtils.GetFormattedLocalNow();
        Status = status;
        Message = message;
        Data = data;
        Errors = errors;
    }

    public static RestResponse<T> Success(int status, string message, T data) =>
        new(status, message, data);

    public static RestResponse<T> Error(int status, string message, List<ErrorDetails> errors) =>
        new(status, message, default, errors);
}