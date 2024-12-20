﻿using Dima.Api.Common.Api;
using Dima.Api.Models;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Category;
using Dima.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dima.Api.Common.Endpoints.Categories
{
    public class UpdateCategoryEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapPut("/{id}", HandleAsync)
            .WithName("Categories: Update")
            .WithSummary("Atualiza uma categoria")
            .WithDescription("Atualiza uma categoria")
            .WithOrder(2)
            .Produces<Response<Category?>>();
        }

        private static async Task<IResult> HandleAsync(
            ClaimsPrincipal user,
            ICategoryHandler handler, 
            UpdateCategoryRequest request, 
             long id)
        {
            request.UserId = user.Identity?.Name ?? string.Empty;
            request.Id = id;
            var result = await handler.UpdateAsync(request);
            return result.IsSuccess 
                ? TypedResults.Ok(result) 
                : TypedResults.BadRequest(result);
        }
    }
}
