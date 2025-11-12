using System.Net;

namespace RestaurantAPI;

internal record ApiResponse(HttpStatusCode? statusCode, string content = "", string errMsg = "")
{
    public HttpStatusCode? StatusCode = statusCode;
    public string Content = content;
    public string ErrorMessage = errMsg;
}
