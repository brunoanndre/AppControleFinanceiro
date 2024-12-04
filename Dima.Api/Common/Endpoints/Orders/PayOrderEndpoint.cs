using System.Security.Claims;
using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;

namespace Dima.Api.Common.Endpoints.Orders;

public class PayOrderEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/{id}/pay", HandleAsync)
            .WithName("Orders: Pay an Order")
            .WithSummary("Pay an Order")
            .WithDescription("Pay an Order")
            .WithOrder(3)
            .Produces<Response<Order?>>();

    private static async Task<IResult> HandleAsync(IOrderHandler handler, ClaimsPrincipal user, long id,
        PayOrderRequest request)
    {
        request.Id = id;
        request.UserId = user.Identity!.Name ?? string.Empty;

        var result = await handler.PayAsync(request);
        
        return result.IsSuccess ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }
}