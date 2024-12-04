using System.Security.Claims;
using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;

namespace Dima.Api.Common.Endpoints.Orders;

public class RefundOrderEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/{id}/refund", HandleAsync)
            .WithName("Orders: Refund and order")
            .WithDescription("Refund an order")
            .Produces<Response<Order?>>();

    private static async Task<IResult> HandleAsync(IOrderHandler handler, long id, ClaimsPrincipal user)
    {
        var request = new RefundOrderRequest
        {
            Id = id,
            UserId = user.Identity!.Name ?? string.Empty,
        };
        
        var result = await handler.RefundAsync(request);
        
        return result.IsSuccess 
            ? TypedResults.Ok(result) 
            : TypedResults.BadRequest(result);
    }
}