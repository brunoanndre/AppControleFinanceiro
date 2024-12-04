using System.Security.Claims;
using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;

namespace Dima.Api.Common.Endpoints.Orders;

public class GetOrderByNumberEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{number}", HandleAsync)
            .WithName("Orders: Get By number")
            .WithDescription("Returns the order by number")
            .WithOrder(6)
            .Produces<Response<Order?>>();

    private static async Task<IResult> HandleAsync(IOrderHandler handler, string number, ClaimsPrincipal user)
    {
        var request = new GetOrderByNumberRequest
        {
            UserId = user.Identity!.Name ?? string.Empty,
            Number = number
        };

        var result = await handler.GetByNumberAsync(request);
        
        return result.IsSuccess ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }
}