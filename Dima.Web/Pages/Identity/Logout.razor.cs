using Dima.Core.Handlers;
using Dima.Web.Security;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Dima.Web.Pages.Identity
{
    public partial class LogoutPage : ComponentBase
    {
        #region Dependences
        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;
        [Inject]
        public IAccountHandler Handler { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public ICookieAuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
        #endregion


        protected override async Task OnInitializedAsync()
        {
            if(await AuthenticationStateProvider.CheckAuthenticatedAsync())
            {
                await Handler.LogoutAsync(); //logout
                await AuthenticationStateProvider.GetAuthenticationStateAsync(); //atualizar o estado de autenticação do usuario
                AuthenticationStateProvider.NotifyAuthenticationStateChanged(); //notificar a aplicação a alteração de estado da autenticação do usuario
            }

            await base.OnInitializedAsync();
        }
    }
}
