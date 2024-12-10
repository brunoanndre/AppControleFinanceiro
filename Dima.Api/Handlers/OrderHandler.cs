using Dima.Api.Data;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Models.Enums;
using Dima.Core.Requests.Orders;
using Dima.Core.Responses;
using Microsoft.EntityFrameworkCore;

namespace Dima.Api.Handlers;

public class OrderHandler(AppDbContext context) : IOrderHandler
{
    public async Task<Response<Order?>> CancelAsync(CancelOrderRequest request)
    {
        Order? order;
        try
        {
            order = await context.Orders
                .Include(o => o.Product)
                .Include(x => x.Voucher)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);
            
            if(order is null) return new Response<Order?>(null, 404, "Pedido não encontrado");
        }
        catch
        {
            return new Response<Order?>(null, 500, "Falha ao obter o pedido");
        }
        
        switch (order.Status)
        {
            case EOrderStatus.Canceled:
                return new Response<Order?>(null, 400, "Este pedido já foi cancelado");
            case EOrderStatus.WaitingPayment:
                break;
            case EOrderStatus.Paid:
                return new Response<Order?>(null, 400, "Este pedido já foi pago e não pode ser cancelado");
            case EOrderStatus.Refunded:
                return new Response<Order?>(null, 400, "Este pedido já foi reembolsado e não pode ser cancelado");
            default:
                return new Response<Order?>(null, 400, "Este pedido não pode ser cancelado");
        }
        
        order.Status = EOrderStatus.Canceled;
        order.UpdatedAt = DateTime.Now;

        try
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
        catch
        {
            return new Response<Order?>(order, 500, "Não foi possível cancelar seu pedido");
        }
        return new Response<Order?>(order, 200, $"Pedido {order.Number} cancelado com sucesso");
    }

    public async Task<Response<Order?>> CreateAsync(CreateOrderRequest request)
    {
        Product? product;
        try
        {
            product = await context
                .Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ProductId && x.IsActive == true);

            if (product is null) 
                return new Response<Order?>(null, 404, "Produto não encontrado");

            context.Attach(product); //Dessa maneira não irá tentar criar o produto novamente no banco
        }
        catch
        {
            return new Response<Order?>(null, 500, "Não foi possível obter o produto");
        }
        
        Voucher? voucher = null;
        try
        {
            if (request.VoucherId is not null)
            {
                voucher = await context
                    .Voucher
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == request.VoucherId && x.IsActive == true);

                if (voucher is null)
                    return new Response<Order?>(null, 400, "Voucher inválido ou não encontrado");
                
                if(voucher.IsActive == false)
                    return new Response<Order?>(null, 400, "Este voucher já foi utilizado");

                voucher.IsActive = false;
                
                context.Voucher.Update(voucher);
            }
        }
        catch
        {
            return new Response<Order?>(null, 500, "Não foi possível obter o voucher");
        }

        var order = new Order
        {
            UserId = request.UserId,
            Product = product,
            ProductId = request.ProductId,
            Voucher = voucher,
            VoucherId = request.VoucherId,
        };

        try
        {
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();
        }
        catch
        {
            return new Response<Order?>(null, 500, "Não foi possível cadastrar o seu pedido");
        }
        
        return new Response<Order?>(order, 201, $"Pedido {order.Number} cadastrado com sucesso");
    }

    public async Task<Response<Order?>> PayAsync(PayOrderRequest request)
    {
        Order? order;
        try
        {
            order = await context.Orders
                .Include(o => o.Product)
                .Include(x => x.Voucher)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (order is null)
                return new Response<Order?>(null, 404, "Pedido não encontrado");
        }
        catch
        {
            return new Response<Order?>(null, 500, "Falha ao consultar o pedido");
        }

        switch (order.Status)
        {
            case EOrderStatus.Canceled:
                return new Response<Order?>(order, 400, "Este pedido já foi cancelado e não pode ser pago");
            case EOrderStatus.Paid:
                return new Response<Order?>(order, 400, "Este pedido já está pago.");
            case EOrderStatus.Refunded:
                return new Response<Order?>(order, 400, "Este pedido já foi reembolsado e não pode ser pago");
            case EOrderStatus.WaitingPayment:
                break;
            default:
                return new Response<Order?>(order, 400, "Não foi possível pagar o pedido");
        }

        order.Status = EOrderStatus.Paid;
        order.ExternalReference = request.ExternalReference;
        order.UpdatedAt = DateTime.Now;

        try
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
        catch
        {
            return new Response<Order?>(order, 500, "Falha ao tentar pagar o pedido");
        }
        
        return new Response<Order?>(order, 200, $"Pedido {order.Number} pago com sucesso");
    }

    public async Task<Response<Order?>> RefundAsync(RefundOrderRequest request)
    {
        Order? order;
        try
        {
            order = await context.Orders
                .Include(o => o.Product)
                .Include(x => x.Voucher)
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (order is null)
                return new Response<Order?>(null, 404, "Pedido não encontrado");
        }
        catch
        {
            return new Response<Order?>(null, 500, "Não foi possível obter o pedido");
        }

        switch (order.Status)
        {
            case EOrderStatus.Canceled:
                return new Response<Order?>(order, 400, "Este pedido já foi cancelado e não pode ser reembolsado");
            case EOrderStatus.Paid:
                break;
            case EOrderStatus.Refunded:
                return new Response<Order?>(order, 400, "Este pedido já foi reembolsado");
            case EOrderStatus.WaitingPayment:
                return new Response<Order?>(order, 400, "Este pedido ainda não foi pago e não pode ser reembolsado");
            default:
                return new Response<Order?>(order, 400, "Não foi possível reembolsar o pedido");
        }
        
        order.Status = EOrderStatus.Refunded;
        order.UpdatedAt = DateTime.Now;

        try
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }
        catch
        {
            return new Response<Order?>(order, 500, "Falha ao tentar reembolsar o pedido");
        }
        
        return new Response<Order?>(order, 200, $"Pedido {order.Number} reembolsado com sucesso");
    }

    public async Task<PagedResponse<List<Order>?>> GetAllAsync(GetAllOrdersRequest request)
    {
        try
        {
            var query = context.Orders.AsNoTracking()
                .Include(x => x.Product)
                .Include(x => x.Voucher)
                .Where(x => x.UserId == request.UserId)
                .OrderByDescending(x => x.CreatedAt);

            var orders = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var count = await query.CountAsync();

            return new PagedResponse<List<Order>?>(orders, count, request.PageNumber, request.PageSize);
        }
        catch
        {
            return new PagedResponse<List<Order>?>(null, 500, "Não foi possível obter os pedidos");
        }
    }

    public async Task<Response<Order?>> GetByNumberAsync(GetOrderByNumberRequest request)
    {
        try
        {
            var order = await context.Orders.AsNoTracking()
                .Include(x => x.Product)
                .Include(x => x.Voucher)
                .FirstOrDefaultAsync(x => x.UserId == request.UserId && x.Number == request.Number);

            if (order is null)
                return new Response<Order?>(null, 404, "Pedido não encontrado");

            return new Response<Order?>(order, 200);
        }
        catch
        {
            return new Response<Order?>(null, 500, "Não foi possível obter o seu pedido");
        }
    }
}