using Dima.Core.Handlers;
using Dima.Core.Requests.Account;
using Dima.Web.Security;
using Microsoft.AspNetCore.Components;
using MudBlazor;


namespace Dima.Web.Pages.Identity
{
    public partial class RegisterPage : ComponentBase
    {
        #region Properties
        public bool IsBusy {  get; set; }
        public RegisterRequest InputModel { get; set; } = new();
        #endregion

        #region Dependences
            [Inject]
            public ISnackbar Snackbar { get; set; } = null!;
            [Inject]
            public IAccountHandler accountHandler { get; set; } = null!;

            [Inject]
            public NavigationManager navigationManager { get; set; } = null!;

            [Inject]
            public ICookieAuthenticationStateProvider authenticationStateProvider { get; set; } = null!;
        #endregion

        protected override async Task OnInitializedAsync()
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not null && user.Identity.IsAuthenticated) navigationManager.NavigateTo("/");
        }


        public async Task OnValidSubmitAsync()
        {
            IsBusy = true;

            try
            {
                var result = await accountHandler.RegisterAsync(InputModel);

                if (result.IsSuccess)
                {
                    navigationManager.NavigateTo("/login");
                    Snackbar.Add(result.Message, Severity.Success);
                }else 
                    Snackbar.Add(result.Message,Severity.Error);

            }catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
