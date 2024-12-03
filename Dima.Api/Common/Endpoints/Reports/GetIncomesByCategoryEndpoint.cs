using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models.Reports;
using Dima.Core.Requests.Reports;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.Api.Common.Endpoints.Reports
{
    public class GetIncomesByCategoryEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/incomes", HandleAsync)
            .Produces<Response<List<IncomesByCategory>>>();
        }

        private static async Task<IResult> HandleAsync(ClaimsPrincipal user, IReportHandler Handler)
        {
            var request = new GetIncomesByCategoryRequest
            {
                UserId = user.Identity?.Name ?? string.Empty
            };

            var result = await Handler.GetIncomesByCategoryReportAsync(request);

            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}
