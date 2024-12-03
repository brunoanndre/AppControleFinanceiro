using Dima.Api.Common.Api;
using Dima.Api.Models;
using Dima.Core;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Category;
using Dima.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dima.Api.Common.Endpoints.Categories
{
    public class GetAllCategoriesEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/", HandleAsync)
                .WithName("Categories: Get All")
                .WithSummary("Retornar todas as categories")
                .WithDescription("Retornar todas as categories")
                .WithOrder(5)
                .Produces<PagedResponse<List<Category>?>>();
        }

        private static async Task<IResult> HandleAsync(
            ClaimsPrincipal user,
            ICategoryHandler handler, 
            [FromQuery] int pageSize = Configuration.DefaultPageSize,
            [FromQuery] int pageNumber = Configuration.DefaultPageNumber)
        {
            var request = new GetAllCategoriesRequest
            {
                UserId = user.Identity?.Name ?? string.Empty,
                PageSize = pageSize,
                PageNumber = pageNumber
            };

            var result = await handler.GetAllAsync(request);
            return result.IsSuccess 
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}
