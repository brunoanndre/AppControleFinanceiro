using Dima.Api.Data;
using Dima.Core.Common.Extensions;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Models.Enums;
using Dima.Core.Requests.Transaction;
using Dima.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Handlers
{
    public class TransactionHandler(AppDbContext context) : ITransactionHandler
    {
        public async Task<Response<Transaction?>> CreateAsync(CreateTransactionRequest request)
        {
            if (request is { Type: ETransactionType.Withdraw, Amount: >= 0 })
            {
                request.Amount *= -1;
            }
            try
            {
                var transaction = new Transaction
                {
                    UserId = request.UserId,
                    Title = request.Title,
                    Type = request.Type,
                    CreatedAt = DateTime.Now,
                    Amount = request.Amount,
                    PaidOrReceivedAt = request.PaidOrReceivedAt,
                    CategoryId = request.CategoryId,
                };
                await context.Transactions.AddAsync(transaction);
                await context.SaveChangesAsync();
                return new Response<Transaction?>(transaction,201,"Transação criada com sucesso."); ;
            }
            catch
            {
                return new Response<Transaction?>(null,500,"Não foi possível cadastrar a transação.");
            }
        }

        public async Task<Response<Transaction?>> DeleteAsync(DeleteTransactionRequest request)
        {
            try
            {
                var transaction = await context
                    .Transactions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == request.Id && t.UserId == request.UserId);

                if (transaction is null) return new Response<Transaction?>(null, 404, "Transação não encontrada");

                    context.Transactions.Remove(transaction);
                    await context.SaveChangesAsync();
                return new Response<Transaction?>(null,200,"Transação excluída com sucesso");
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Não foi possível excluir a transação");
            }
        }

        public async Task<PagedResponse<List<Transaction>?>> GetByPeriodAsync(GetTransactionByPeriodRequest request)
        {
            try
            {
                request.InitialDate ??= DateTime.Now.GetFirstDay();
                request.EndDate ??= DateTime.Now.GetLastDay();

                var query = context
                    .Transactions
                    .AsNoTracking()
                    .Where(t => t.UserId == request.UserId && t.PaidOrReceivedAt >= request.InitialDate && t.PaidOrReceivedAt <= request.EndDate)
                    .OrderBy(t => t.CreatedAt);

                var transactions = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToListAsync();
                var count = await query.CountAsync();

                return new PagedResponse<List<Transaction>?>(transactions,count,request.PageNumber,request.PageSize);
            }
            catch
            {
                return new PagedResponse<List<Transaction>?>(null, 500, "Não foi possível retornar as transações");
            }
        }

        public async Task<Response<Transaction?>> GetByIdAsync(GetTransactionByIdRequest request)
        {
            try
            {
                var transaction = await context
                    .Transactions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == request.Id && t.UserId == request.UserId);

                return transaction is null
                    ? new Response<Transaction?>(null, 404, "Transação não encontrada")
                    : new Response<Transaction?>(transaction, 200);
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Não foi possível retornar a transação");
            }
        }

        public async Task<Response<Transaction?>> UpdateAsync(UpdateTransactionRequest request, long id)
        {
            if (request is { Type: ETransactionType.Withdraw, Amount: >= 0 })
            {
                request.Amount *= -1;
            }
            try
            {
                var transaction = await context
                    .Transactions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id && t.UserId == request.UserId);

                if (transaction is null) return new Response<Transaction?>(null, 404, "Transação não encontrada");

                transaction.Title = request.Title;
                transaction.Amount = request.Amount;
                transaction.Type = request.Type;
                transaction.CategoryId = request.CategoryId;
                transaction.PaidOrReceivedAt = request.PaidOrReceivedAt;

                context.Transactions.Update(transaction);
                await context.SaveChangesAsync();
                return new Response<Transaction?>(null, 200, "Transação atualizada com sucesso");
            }
            catch
            {
                return new Response<Transaction?>(null, 500, "Não foi possível atualizar a transação");
            }
        }
    }
}
