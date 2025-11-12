using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Sms.Test;

namespace RestaurantgRPC;

public class RestaurantGrpcClient : IAsyncDisposable
{
    private readonly GrpcChannel _channel;
    private readonly SmsTestService.SmsTestServiceClient _client;

    public RestaurantGrpcClient(string url)
    {
        _channel = GrpcChannel.ForAddress(url);
        _client = new SmsTestService.SmsTestServiceClient(_channel);
    }

    public async Task<GetMenuResponse> GetMenuAsync(bool withPrice)
    {
        var request = new BoolValue { Value = withPrice };
        return await _client.GetMenuAsync(request);
    }

    public async Task<SendOrderResponse> SendOrderAsync(Order order)
    {
        return await _client.SendOrderAsync(order);
    }

    public async ValueTask DisposeAsync()
    {
        await _channel.ShutdownAsync();
    }
}
