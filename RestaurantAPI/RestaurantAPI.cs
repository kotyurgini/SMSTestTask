using Newtonsoft.Json;
using RestaurantAPI.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using static System.Net.Http.HttpMethod;

namespace RestaurantAPI;

public class RestaurantAPI
{
    private readonly HttpClient client;

    public RestaurantAPI(string baseUrl, string login, string password)
    {
        client = new()
        {
            BaseAddress = new(baseUrl),
            Timeout = TimeSpan.FromSeconds(10)
        };
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(Encoding.ASCII.GetBytes($"{login}:{password}")));
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Terminal", "1.0"));
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<ResponseModel<GetMenuResponse>> GetMenuAsync(bool withPrice = true)
    {
        return HandleHttpResponse<GetMenuResponse>(await SendRequestAsync("restaurant", Post, JsonConvert.SerializeObject(new RequestModel<GetMenuParamenters>()
        {
            Command = "GetMenu",
            CommandParamenters = new()
            {
                WithPrice = withPrice
            }
        })));
    }

    public async Task<ResponseModel> SendOrderAsync(string orderId, List<CartItem> cart)
    {
        return HandleHttpResponse(await SendRequestAsync("restaurant", Post, JsonConvert.SerializeObject(new RequestModel<SendOrderParamenters>()
        {
            Command = "SendOrder",
            CommandParamenters = new()
            {
                OrderId = orderId,
                MenuItems = cart
            }
        })));
    }

    private async Task<ApiResponse> SendRequestAsync(string endpoint, HttpMethod method, string data)
    {
        try
        {
            using var request = new HttpRequestMessage(method, endpoint);

            using var content = new StringContent(data, Encoding.UTF8, "application/json");
            if (method == Post || method == Put) request.Content = content;

            using var response = await client.SendAsync(request);
            var str = await response.Content.ReadAsStringAsync();

            if (response.StatusCode != HttpStatusCode.OK)
                return new(response.StatusCode, errMsg: "Некорректный статус код ответа от сервера");

            return new(response.StatusCode, str);
        }
        catch
        {
            return new(null, errMsg: "Ошибка соединения с сервером");
        }
    }

    private static ResponseModel HandleHttpResponse(ApiResponse response)
    {
        if (response.ErrorMessage is { Length: > 0 })
            return new() { ErrorMessage = response.ErrorMessage };

        try
        {
            if (string.IsNullOrEmpty(response.Content))
                return new() { ErrorMessage = "Некорректный ответ от серера" };
            var result = JsonConvert.DeserializeObject<ResponseModel>(response.Content)!;
            return result;
        }
        catch
        {
            return new() { ErrorMessage = "Не удалось преобразовать ответ от сервера" };
        }
    }

    private static ResponseModel<T> HandleHttpResponse<T>(ApiResponse response)
    {
        if (response.ErrorMessage is { Length: > 0 })
            return new() { ErrorMessage = response.ErrorMessage };

        try
        {
            if (string.IsNullOrEmpty(response.Content))
                return new() { ErrorMessage = "Некорректный ответ от серера" };
            var result = JsonConvert.DeserializeObject<ResponseModel<T>>(response.Content)!;
            return result;
        }
        catch
        {
            return new() { ErrorMessage = "Не удалось преобразовать ответ от сервера" };
        }
    }
}
