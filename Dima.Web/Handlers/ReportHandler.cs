using System.Net;
using Dima.Core.Handlers;
using Dima.Core.Models.Reports;
using Dima.Core.Requests.Reports;
using Dima.Core.Responses;
using System.Net.Http.Json;

namespace Dima.Web.Handlers
{
    public class ReportHandler(IHttpClientFactory httpClientFactory) : IReportHandler
    {

        private readonly HttpClient _httpClient =httpClientFactory.CreateClient(Configuration.HttpClientName) ;
        public async Task<Response<List<ExpensesByCategory>?>> GetExpensesByCategoryReportAsync(GetExpensesByCategoryRequest request)
        {
            return await _httpClient.GetFromJsonAsync<Response<List<ExpensesByCategory>?>>($"v1/reports/expenses")
                 ?? new Response<List<ExpensesByCategory>?>(null, 400, "Não foi possível obter os dados");
        }

        public async Task<Response<FinancialSummary?>> GetFinancialSummaryReportAsync(GetFinancialSummaryRequest request)
        {
            var httpResponse = await _httpClient.GetAsync("v1/reports/summary");

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = await httpResponse.Content.ReadFromJsonAsync<Response<FinancialSummary?>>();

                return response!.IsSuccess
                    ? new Response<FinancialSummary?>(response.Data, 200)
                    : new Response<FinancialSummary?>(null, 400, "Erro ao obter o resumo financeiro");
            }
            else if(httpResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return new Response<FinancialSummary?>(null, 404, "Não foram encontrados registros financeiros");
            }
            
            return new Response<FinancialSummary?>(null, 400, "Erro ao obter o resumo financeiro");

            //return await _httpClient.GetFromJsonAsync<Response<FinancialSummary?>>("v1/reports/summary")
            // ?? new Response<FinancialSummary?>(null, 400, "Não foi possível obter os dados");
        }

        public async Task<Response<List<IncomesAndExpenses>?>> GetIncomesAndExpensesReportAsync(GetIncomesAndExpensesRequest request)
        {
            return await _httpClient.GetFromJsonAsync<Response<List<IncomesAndExpenses>?>>("v1/reports/incomes-expenses")
                ?? new Response<List<IncomesAndExpenses>?>(null, 400, "Não foi possível obter os dados");
        }

        public async Task<Response<List<IncomesByCategory>?>> GetIncomesByCategoryReportAsync(GetIncomesByCategoryRequest request)
        {
            return await _httpClient.GetFromJsonAsync<Response<List<IncomesByCategory>?>>("v1/reports/incomes")
    ?? new Response<List<IncomesByCategory>?>(null, 400, "Não foi possível obter os dados");
        }
    }
}
