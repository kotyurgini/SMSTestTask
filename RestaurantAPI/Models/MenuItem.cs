namespace RestaurantAPI.Models;

public class MenuItem
{
    public string Id { get; set; } = "";
    public string Article { get; set; } = "";
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public bool IsWeighted { get; set; }
    public string FullPath { get; set; } = "";
    public List<string> Barcodes { get; set; } = [];
}

public class CartItem
{
    public string Id { get; set; } = "";
    public string Quantity { get; set; } = "";
}
