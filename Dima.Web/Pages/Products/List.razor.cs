using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Orders;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Products;

public partial class ListProductsPage : ComponentBase
{
    #region Properties

    public bool IsBusy { get; set; }
    public List<Product> Products { get; set; } = [];
    #endregion
    
    #region Services

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IProductHandler Handler { get; set; } = null!;
    #endregion
    
    #region Overrides

    protected async override Task OnInitializedAsync()
    {
        IsBusy = true;
        try
        {
            var request = new GetAllProductsRequest();
            
            var response = await Handler.GetAllAsync(request);

            if (response.IsSuccess)
                Products = response.Data ?? [];
            else
            {
                Snackbar.Add(response.Message, Severity.Error);
            }
        }
        catch(Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
        finally
        {
            IsBusy = false;
        }
    }

    #endregion
}