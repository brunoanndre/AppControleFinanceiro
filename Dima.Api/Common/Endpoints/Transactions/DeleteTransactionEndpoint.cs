﻿using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Transaction;
using Dima.Core.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dima.Api.Common.Endpoints.Transactions
{
    public class DeleteTransactionEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapDelete("/{id}",HandleAsync)
                .WithName("Transactions: Delete")
                .WithSummary("Excluir transação")
                .WithDescription("Excluir transação")
                .WithOrder(3)
                .Produces<Response<Transaction>>();
        }

        private static async Task<IResult> HandleAsync(
            ClaimsPrincipal user,
            ITransactionHandler handler,
            long id)
        {
            var request = new DeleteTransactionRequest
            {
                Id = id,
                UserId = user.Identity?.Name ?? string.Empty
        };
            var result = await handler.DeleteAsync(request);
            return result.IsSuccess
                    ? TypedResults.Ok(result)
                    : TypedResults.BadRequest(result);
        }
    }
}
