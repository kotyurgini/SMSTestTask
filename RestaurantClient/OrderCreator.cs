using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Models;
using RestaurantClient.Database;

namespace RestaurantClient;
public class OrderCreator(ILogger<OrderCreator> log, IConfiguration config)
{
    public async Task Run()
    {
        try
        {
            var dbConStr = config.GetValue<string>("DbConnectionString") ?? "";
            await DbMethods.DbEnsureCreatedAsync(dbConStr);

            var api = new RestaurantAPI.RestaurantAPI(
                config.GetValue<string>("RestaurantHost") ?? "",
                config.GetValue<string>("RestaurantLogin") ?? "",
                config.GetValue<string>("RestaurantPassword") ?? "");

            var resp = await api.GetMenuAsync();
            if (!resp.Success)
            {
                LogConsoleMessage($"Ошибка получения списка блюд. {resp.ErrorMessage}");
                return;
            }
            if (resp.Data is null || resp.Data.MenuItems.Count == 0)
            {
                LogConsoleMessage($"Пришел некорректный или пустой список блюд");
                return;
            }
            var menu = resp.Data.MenuItems;
            await DbMethods.SaveMenuToDb(dbConStr, menu);
            ShowMenu(menu);

            var orderId = $"{Guid.NewGuid()}";
            var orderItems = MakeOrder(menu);

            var oResp = await api.SendOrderAsync(orderId, orderItems);
            if (oResp.Success)
                LogConsoleMessage("УСПЕХ");
            else
                LogConsoleMessage($"Ошибка создания заказа. {oResp.ErrorMessage}");
        }
        catch (Exception ex)
        {
            LogConsoleMessage($"Ошибка во время выполнения.\n{ex}");
        }
    }

    private void LogConsoleMessage(string msg)
    {
        Console.WriteLine(msg);
        log.LogInformation("[CONSOLE]: {msg}", msg);
    }

    private void ShowMenu(List<MenuItem> menu)
    {
        var text = $"Меню:";
        foreach (var item in menu)
            text += $"\n{item.Name} - {item.Article} - {item.Price}";
        LogConsoleMessage($"{text}\n");
    }

    private List<CartItem> MakeOrder(List<MenuItem> menu)
    {
        List<CartItem> cart = [];
        LogConsoleMessage("Введите заказ в формате \'Код1:Количество1;Код2:Количество2;\'");

        while (true)
        {
            var error = false;
            cart = [];
            var input = Console.ReadLine();
            if (input is not { Length: > 0 })
            {
                LogConsoleMessage("Необходимо указать позиции заказа в формате 'Код1:Количество1;Код2:Количество2;'. Повторите ввод\n");
                continue;
            }
            var positions = input.Trim().Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var position in positions)
            {
                var pos = position.Split(':', StringSplitOptions.RemoveEmptyEntries);
                if (pos.Length != 2)
                {
                    LogConsoleMessage("Необходимо указать позиции заказа в формате 'Код1:Количество1;Код2:Количество2;'. Повторите ввод\n");
                    error = true;
                    break;
                }
                var menuItem = menu.FirstOrDefault(x => x.Article == pos[0]);
                if (menuItem is null)
                {
                    LogConsoleMessage($"В меню нет позиции с кодом \'{pos[0]}\'. Повторите ввод\n");
                    error = true;
                    break;
                }
                if (!decimal.TryParse(pos[1], out var q) || q <= 0)
                {
                    LogConsoleMessage($"Некорректное кол-во \'{pos[1]}\'. Повторите ввод\n");
                    error = true;
                    break;
                }
                cart.Add(new()
                {
                    Id = menuItem.Id,
                    Quantity = pos[1]
                });
            }
            if (error) continue;
            return cart;
        }
    }
}
