namespace SWP391_Project.Dtos;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = null!;
    public List<T> Data { get; set; } = new();
}
