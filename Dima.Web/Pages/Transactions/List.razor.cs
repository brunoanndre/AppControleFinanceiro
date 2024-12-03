using Dima.Core.Common.Extensions;
using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Transaction;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Transactions
{
    public partial class ListTransactionPage : ComponentBase
    {
        #region Properties

        public bool IsBusy { get; set; } = false;
        public List<Transaction> Transactions { get; set; } = [];
        public string SearchTerm { get; set; } = string.Empty;
        public int CurrentYear { get; set; } = DateTime.Now.Year;
        public int CurrentMonth { get; set; } = DateTime.Now.Month;

        public int[] Years { get; set; } =
        {
            DateTime.Now.Year,
            DateTime.Now.AddYears(-1).Year,
            DateTime.Now.AddYears(-2).Year,
            DateTime.Now.AddYears(-3).Year
        };

        #endregion

        #region Services

        [Inject] 
        public ISnackbar SnackBar { get; set; } = null!;

        [Inject] 
        public ITransactionHandler Handler { get; set; } = null!;
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject] 
        public IDialogService DialogService { get; set; } = null!;

        #endregion
        #region Overrides

        protected override async Task OnInitializedAsync()
        {
            await GetTransactionsAsync();
        }

        #endregion
        #region Methods
        private async Task GetTransactionsAsync()
        {
            try
            {
                IsBusy = true;
                var request = new GetTransactionByPeriodRequest
                {
                    InitialDate = DateTime.Now.GetFirstDay(CurrentYear,CurrentMonth),
                    EndDate = DateTime.Now.GetLastDay(CurrentYear,CurrentMonth),
                    PageNumber = 1,
                    PageSize = 1000
                };
                var result = await Handler.GetByPeriodAsync(request);

                if (result.IsSuccess)
                {
                    Transactions = result.Data ?? [];
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

        public async Task OnSearchAsync()
        {
            await GetTransactionsAsync();
            StateHasChanged();
        }
        public Func<Transaction, bool> Filter => transaction =>
        {
            if (string.IsNullOrEmpty(SearchTerm)) return true;

            return transaction.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase)
                   || transaction.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase);
        };


        public async void OnDeleteButtonClickedAsync(long id, string title)
        {
            var result = await DialogService.ShowMessageBox("Atenção", $"Ao prosseguir a transação {title} será excluída. Esta é uma ação irreversível, deseja continuar?", yesText: "Excluir", cancelText: "Cancelar");

            if (result is true)
            {
                await OnDeleteAsync(id, title);
            }
            StateHasChanged();
        }

        public async Task OnDeleteAsync(long id, string title)
        {
            try
            {
                IsBusy = true;
                var request = new DeleteTransactionRequest
                {
                    Id = id
                };
                var result = await Handler.DeleteAsync(request);
                if (result.IsSuccess)
                {
                    SnackBar.Add($"Transação {title} excluída com sucesso", Severity.Success);
                    Transactions.RemoveAll(x => x.Id == id);
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
