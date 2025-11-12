namespace RestaurantAPI.Models;

public class GetMenuParamenters
{
    public bool WithPrice { get; set; }
}

public class SendOrderParamenters
{
    public string OrderId { get; set; } = "";
    public List<CartItem> MenuItems { get; set; } = [];
}