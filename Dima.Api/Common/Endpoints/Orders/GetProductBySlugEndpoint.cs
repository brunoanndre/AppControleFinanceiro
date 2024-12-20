using System.Security.Claims;
using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;

namespace Dima.Api.Common.Endpoints.Orders;

public class GetProductBySlugEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapGet("/{slug}", HandleAsync)
        .WithName("Products: Get Product By Slug")
        .WithDescription("Returns product by Slug")
        .WithSummary("Returns product by Slug")
        .WithOrder(2)
        .Produces<Response<Product?>>();

    private static async Task<IResult> HandleAsync(IProductHandler handler, string slug)
    {
        var request = new GetProductBySlugRequest
        {
            Slug = slug
        };

        var result = await handler.GetBySlugAsync(request);
        
        return result.IsSuccess ? TypedResults.Ok(result) : TypedResults.BadRequest(result);
    }
}