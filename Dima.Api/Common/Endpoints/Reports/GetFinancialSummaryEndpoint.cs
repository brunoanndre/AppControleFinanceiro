using Dima.Api.Common.Api;
using Dima.Core.Handlers;
using Dima.Core.Models.Reports;
using Dima.Core.Requests.Reports;
using Dima.Core.Responses;
using System.Security.Claims;

namespace Dima.Api.Common.Endpoints.Reports
{
    public class GetFinancialSummaryEndpoint : IEndpoint
    {
        public static void Map(IEndpointRouteBuilder app)
        {
            app.MapGet("/summary", HandleAsync)
                .WithName("Financial summary")
                .Produces<Response<FinancialSummary>>();

        }

        public static async Task<IResult> HandleAsync(ClaimsPrincipal user, IReportHandler Handler)
        {
            var request = new GetFinancialSummaryRequest
            {
                UserId = user.Identity?.Name ?? string.Empty
            };

            var result = await Handler.GetFinancialSummaryReportAsync(request);

            return result!.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.BadRequest(result);
        }
    }
}
