namespace RestaurantAPI.Models;

public class ResponseModel
{
    public string Command { get; set; } = "";
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = "";
}

public class ResponseModel<T> : ResponseModel
{
    public T? Data { get; set; }
}