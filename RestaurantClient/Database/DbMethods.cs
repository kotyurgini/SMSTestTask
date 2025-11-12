using RestaurantAPI.Models;
using RestaurantClient.Database.Models;

namespace RestaurantClient.Database;
public static class DbMethods
{
    public static async Task DbEnsureCreatedAsync(string conString)
    {
        using var db = new PostgreContext(conString);
        await db.Database.EnsureCreatedAsync();
    }

    public static async Task SaveMenuToDb(string conString, List<MenuItem> menu)
    {
        using var db = new PostgreContext(conString);
        var ids = db.MenuItems.Select(x => x.Id).ToList();

        foreach (var item in menu)
        {
            if (ids.Contains(item.Id))
                db.MenuItems.Update(new DbMenuItem()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Article = item.Article,
                    Barcodes = item.Barcodes,
                    FullPath = item.FullPath,
                    IsWeighted = item.IsWeighted,
                    Price = item.Price
                });
            else
                await db.MenuItems.AddAsync(new DbMenuItem()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Article = item.Article,
                    Barcodes = item.Barcodes,
                    FullPath = item.FullPath,
                    IsWeighted = item.IsWeighted,
                    Price = item.Price
                });
        }
        await db.SaveChangesAsync();
    }
}
