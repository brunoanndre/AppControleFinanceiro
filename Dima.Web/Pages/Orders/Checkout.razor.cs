using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Orders;

public partial class CheckoutPage : ComponentBase
{
    #region Parameters

    [Parameter] public string ProductSlug { get; set; } = string.Empty;
    [SupplyParameterFromQuery(Name = "voucher")] public string? VoucherNumber { get; set; }
    #endregion

    #region Properties

    public PatternMask Mask = new("####-####")
    {
        MaskChars = [new MaskChar('#', @"[0-9a-fA-F]")],
        Placeholder = '_',
        CleanDelimiters = true,
        Transformation = AllUpperCase
    };
    
    public bool IsBusy { get; set; }
    public bool IsValid { get; set; }
    public CreateOrderRequest InpuModel { get; set; } = new();
    public Product? Product { get; set; }
    public Voucher? Voucher { get; set; }
    public decimal Total { get; set; }
    #endregion
    
    #region Services

    [Inject] public IProductHandler ProductHandler { get; set; } = null!;
    [Inject] public IOrderHandler OrderHandler { get; set; } = null!;
    [Inject] public IVoucherHandler VoucherHandler { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    #endregion
    
    #region Methods

    protected override async Task OnInitializedAsync()
    {
        //Recuperar o produto
        try
        {
            var request = new GetProductBySlugRequest
            {
                Slug = ProductSlug
            };
            var result = await ProductHandler.GetBySlugAsync(request);

            if (result.IsSuccess == false)
            {
                Snackbar.Add("Não foi possível obter o produto.", Severity.Error);
                IsValid = false;
                return;
            }
            
            Product = result.Data;
                
        }
        catch
        {
            Snackbar.Add("Não foi possível obter o produto.", Severity.Error);
            IsValid = false;
        }

        if (Product is null)
        {
            Snackbar.Add("Não foi possível obter o produto.", Severity.Error);
            IsValid = false;
            return;
        }
        
        //Recuperar o Voucher
        if (!string.IsNullOrEmpty(VoucherNumber))
        {
            try
            {
                var request = new GetVoucherByNumberRequest
                {
                    Number = VoucherNumber.Replace("-", "")
                };
                
                var result = await VoucherHandler.GetByNumberAsync(request);

                if (!result.IsSuccess)
                {
                    VoucherNumber = string.Empty;
                    Snackbar.Add("Não foi possível obter o voucher.", Severity.Error); 
                }

                if (result.Data is null)
                {
                    VoucherNumber = string.Empty;
                    Snackbar.Add("Não foi possível obter o voucher.", Severity.Error);
                }
                
                Voucher = result.Data;
            }
            catch
            {
                VoucherNumber = string.Empty;
                Snackbar.Add("Não foi possível obter o voucher.", Severity.Error);
            }
        }

        IsValid = true;
        Total = Product.Price - (Voucher?.Amount ?? 0);
    }

    public async Task OnValidSubmitAsync()
    {
        IsBusy = true;
        try
        {
            var request = new CreateOrderRequest
            {
                ProductId = Product!.Id,
                VoucherId = Voucher?.Id ?? null
            };
            
            var result = await OrderHandler.CreateAsync(request);

            if (result.IsSuccess)
            {
                NavigationManager.NavigateTo($"/pedidos/{result.Data!.Number}");
            }
            else
            {
                Snackbar.Add(result.Message, Severity.Error);
            }
        }
        catch(Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);   
        }
        finally
        {
            IsBusy = false;
        }
        
    }

    private static char AllUpperCase(char c) => c.ToString().ToUpperInvariant()[0];

    #endregion
}