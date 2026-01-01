namespace Mess.Api;

public class ApiError(string error)
{
    public string Error { get; set; } = error;
}