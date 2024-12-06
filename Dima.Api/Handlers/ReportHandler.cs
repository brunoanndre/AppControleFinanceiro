using Dima.Api.Data;
using Dima.Core.Handlers;
using Dima.Core.Models.Reports;
using Dima.Core.Requests.Reports;
using Dima.Core.Responses;
using Microsoft.EntityFrameworkCore;
using Dima.Core.Models.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dima.Api.Handlers
{
    public class ReportHandler(AppDbContext context) : IReportHandler
    {
        public async Task<Response<List<ExpensesByCategory>?>> GetExpensesByCategoryReportAsync(GetExpensesByCategoryRequest request)
        {
            try
            {
                var data = await context.ExpensesByCategories
                    .AsNoTracking()
                    .Where(x => x.UserId == request.UserId)
                    .OrderByDescending(x => x.Year)
                    .ThenBy(x => x.Category)
                    .ToListAsync();

                return new Response<List<ExpensesByCategory>?>(data, 200);
            }catch
            {
                return new Response<List<ExpensesByCategory>?>(null, 500,"Não foi possível obter as saídas por categoria");
            }
        }

        public async Task<Response<FinancialSummary>?> GetFinancialSummaryReportAsync(GetFinancialSummaryRequest request)
        {
            try
            {
                // Resumo financeiro do mês
                var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                var data = await context.Transactions.AsNoTracking()
                    .Where(x => x.UserId == request.UserId && x.PaidOrReceivedAt >= startDate && x.PaidOrReceivedAt <= DateTime.Now)
                    .GroupBy(x => true).Select(x => new FinancialSummary(
                        request.UserId,
                        x.Where(x => x.Type == ETransactionType.Deposit).Sum(t => t.Amount),
                        x.Where(x => x.Type == ETransactionType.Withdraw).Sum(x => x.Amount)))
                    .FirstOrDefaultAsync();

                if (data is null) 
                    return new Response<FinancialSummary>(null, 404, "Não há registros financeiros para o mês atual.");

                return new Response<FinancialSummary>(data, 200,"funcionou");
            }
            catch
            {
                return new Response<FinancialSummary>(null, 500, "Não foi possível obter o resultado financeiro");
            }
        }

        public async Task<Response<List<IncomesAndExpenses>?>> GetIncomesAndExpensesReportAsync(GetIncomesAndExpensesRequest request)
        {
            try
            {
                var data = await context.IncomesAndExpenses
                    .AsNoTracking()
                    .Where(x => x.UserId == request.UserId)
                    .OrderByDescending(x => x.Year)
                    .ThenBy(x => x.Month)
                    .ToListAsync();

                return new Response<List<IncomesAndExpenses>?>(data);
            }catch
            {
                return new Response<List<IncomesAndExpenses>?>(null, 500, "Não foi possível obter as entradas e saídas");
            }
        }

        public async Task<Response<List<IncomesByCategory>?>> GetIncomesByCategoryReportAsync(GetIncomesByCategoryRequest request)
        {
            try
            {
                var data = await context.IncomesByCategories
                    .AsNoTracking()
                    .Where(x => x.UserId == request.UserId)
                    .OrderByDescending(x => x.Year)
                    .ThenBy(x => x.Category)
                    .ToListAsync();

                return new Response<List<IncomesByCategory>?>(data,200);
            }catch
            {
                return new Response<List<IncomesByCategory>?>(null, 500, "Não foi possível obter as entradas por categoria");
            }
        }
    }
}
