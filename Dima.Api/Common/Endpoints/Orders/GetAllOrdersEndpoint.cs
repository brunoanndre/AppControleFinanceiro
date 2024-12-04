using System.Security.Claims;
using Dima.Api.Common.Api;
using Dima.Core;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Dima.Api.Common.Endpoints.Orders;

public class GetAllOrdersEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapGet("/",HandleAsync)
        .WithName("Orders: Get all orders")
        .WithSummary("Get all orders")
        .WithOrder(5)
        .Produces<PagedResponse<List<Order>?>>();

    private static async Task<IResult> HandleAsync(IOrderHandler handler, ClaimsPrincipal user, 
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Configuration.DefaultPageSize)
    {
        var request = new GetAllOrdersRequest
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            UserId = user.Identity!.Name ?? string.Empty,
        };

        var result = await handler.GetAllAsync(request);
        
        return result.IsSuccess ? TypedResults.Ok(result) : Results.BadRequest(result);
    }
}