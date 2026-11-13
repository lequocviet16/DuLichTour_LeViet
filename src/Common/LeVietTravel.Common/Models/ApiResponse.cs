using System.Text.Json.Serialization;

namespace LeVietTravel.Common.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public object? Errors { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Pagination { get; set; }

    public static ApiResponse<T> Ok(T? data, string message = "Thành công", object? pagination = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            Pagination = pagination
        };
    }

    public static ApiResponse<T> Fail(string message, object? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

public class ApiResponse : ApiResponse<object>
{
    public static ApiResponse Ok(string message = "Thành công")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }
}
