namespace Models.Common;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public static ApiResponse<T> Ok(T data, string message = "Thành công")
    {
        return new ApiResponse<T>
        {
            Success = true,
            StatusCode = 200,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Fail(string message, int statusCode = 400, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Data = default,
            Errors = errors ?? new List<string>()
        };
    }
}

public static class ApiResponse
{
    public static ApiResponse<object> Ok(object? data = null, string message = "Thành công")
    {
        return new ApiResponse<object>
        {
            Success = true,
            StatusCode = 200,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<object> Fail(string message, int statusCode = 400, List<string>? errors = null)
    {
        return new ApiResponse<object>
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Data = null,
            Errors = errors ?? new List<string>()
        };
    }
}
