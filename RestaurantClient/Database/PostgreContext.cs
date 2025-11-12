using Microsoft.EntityFrameworkCore;
using Npgsql;
using RestaurantClient.Database.Models;

namespace RestaurantClient.Database;
internal class PostgreContext(string conString) : DbContext
{
    public DbSet<DbMenuItem> MenuItems { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder ob)
    {
        var connectionString = conString;

        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        dataSourceBuilder.EnableDynamicJson();
        var dataSource = dataSourceBuilder.Build();
        ob.UseNpgsql(dataSource);
    }

    protected override void OnModelCreating(ModelBuilder mb)
    {
        base.OnModelCreating(mb);

        mb.Entity<DbMenuItem>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Barcodes).HasColumnType("jsonb");
        });
    }
}
