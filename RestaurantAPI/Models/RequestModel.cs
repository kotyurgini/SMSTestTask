namespace RestaurantAPI.Models;

public class RequestModel<T>
{
    public string Command { get; set; } = "";
    public T? CommandParamenters { get; set; }
}
