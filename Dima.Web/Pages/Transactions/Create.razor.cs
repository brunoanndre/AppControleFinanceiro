using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Category;
using Dima.Core.Requests.Transaction;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Transactions
{
    public partial class CreateTransactionPage : ComponentBase
    {
        #region Properties
        public bool IsBusy { get; set; } = false;
        public CreateTransactionRequest InputModel { get; set; } = new();
        public List<Category> Categories { get; set; } = [];

        public List<BreadcrumbItem> Breadcrumb { get; set; } = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Transações", href: "/transacoes"),
            new BreadcrumbItem("Adicionar", href: "#", disabled: true)
        };
        #endregion

        #region Services
        [Inject]
        public ITransactionHandler TransactionHandler { get; set; } = null!;

        [Inject] 
        public ICategoryHandler CategoryHandler { get; set; } = null!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        public ISnackbar SnackBar { get; set; } = null!;
        #endregion

        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsBusy = true;
                var result = await CategoryHandler.GetAllAsync(new GetAllCategoriesRequest());

                if (result.IsSuccess)
                {
                    Categories = result.Data ?? [];
                    InputModel.CategoryId = Categories.FirstOrDefault()?.Id ?? 0;
                    StateHasChanged();
                }
            }
            catch (Exception e)
            {
                SnackBar.Add(e.Message, Severity.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Methods

        public async Task OnValidSubmitAsync()
        {
            IsBusy = true;
            try
            {
                var result = await TransactionHandler.CreateAsync(InputModel);

                if (result.IsSuccess)
                {
                    SnackBar.Add("Transação criada com sucesso", Severity.Success);
                    NavigationManager.NavigateTo("/transacoes");
                }
            }
            catch (Exception e)
            {
                SnackBar.Add(e.Message, Severity.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
        #endregion

    }
}
