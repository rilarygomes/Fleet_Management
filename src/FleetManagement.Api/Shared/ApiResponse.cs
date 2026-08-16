namespace FleetManagement.Api.Common;

public class ApiResponse
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; init; }
}