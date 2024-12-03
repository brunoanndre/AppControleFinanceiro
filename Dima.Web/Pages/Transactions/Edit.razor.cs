using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Category;
using Dima.Core.Requests.Transaction;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Transactions
{
    public partial class EditTransactionPage : ComponentBase
    {
        #region Properties
        public bool IsBusy { get; set; } = false;
        [Parameter]
        public string Id { get; set; }
        public UpdateTransactionRequest InputModel { get; set; } = new();
        public List<Category> Categories { get; set; } = [];

        public List<BreadcrumbItem> Breadcrumb { get; set; } = new List<BreadcrumbItem>
        {
            new BreadcrumbItem("Transações", href: "/transacoes"),
            new BreadcrumbItem("Editar", href: "#", disabled: true)
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
            IsBusy = true;
            await GetTransactionByIdAsync();
            await GetCategoriesAsync();
            IsBusy = false;
        }

        #endregion

        #region Methods

        private async Task GetTransactionByIdAsync()
        {
            IsBusy = true;
            try
            {
                var request = new GetTransactionByIdRequest{Id =  long.Parse(Id)};
                var result = await TransactionHandler.GetByIdAsync(request);

                if (result.IsSuccess)
                {
                    InputModel = new UpdateTransactionRequest
                    {
                        CategoryId = result.Data.CategoryId,
                        PaidOrReceivedAt = result.Data.PaidOrReceivedAt,
                        Title = result.Data.Title,
                        Type = result.Data.Type,
                        Amount = result.Data.Amount,
                    };
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

        private async Task GetCategoriesAsync()
        {
            IsBusy = true;
            try
            {
                var request = new GetAllCategoriesRequest();
                var result = await CategoryHandler.GetAllAsync(request);

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

        public async Task OnValidSubmitAsync()
        {
            IsBusy = true;
            try
            {
                var result = await TransactionHandler.UpdateAsync(InputModel,long.Parse(Id));

                if (result.IsSuccess)
                {
                    SnackBar.Add("Lançamento atualizado com sucesso", Severity.Success);
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
