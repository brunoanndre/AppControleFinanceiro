using Dima.Core.Handlers;
using Dima.Core.Models;
using Dima.Core.Requests.Category;
using Dima.Core.Responses;
using System.Net.Http.Json;

namespace Dima.Web.Handlers
{
    public class CategoryHandler(IHttpClientFactory httpClientFactory) : ICategoryHandler
    {
        private readonly HttpClient _client = httpClientFactory.CreateClient(Configuration.HttpClientName);

        public async Task<Response<Category?>> CreateAsync(CreateCategoryRequest request)
        {
            try
            {
                var result = await _client.PostAsJsonAsync("v1/categories", request);

                return await result.Content.ReadFromJsonAsync<Response<Category?>>()
                    ?? new Response<Category?>(null,400,"Falha ao criar a categoria");   
            }
            catch
            {
                return new Response<Category?>(null, 500, "Um erro inesperado aconteceu, contate o administrador.");
            }
        }

        public async Task<Response<Category?>> DeleteAsync(DeleteCategoryRequest request)
        {
            var result = await _client.DeleteAsync($"v1/categories/{request.Id}");

            return await result.Content.ReadFromJsonAsync<Response<Category?>>()
                ?? new Response<Category?>(null, 400, "Falha ao excluir a categoria");
        }

        public async Task<PagedResponse<List<Category>>> GetAllAsync(GetAllCategoriesRequest request)
        {
            return await _client.GetFromJsonAsync<PagedResponse<List<Category>>>("v1/categories")
                ?? new PagedResponse<List<Category>>(null, 400, "Não foi possível obter as categorias");
        }

        public async Task<Response<Category?>> GetByIdAsync(GetCategoryByIdRequest request)
        {
            return await _client.GetFromJsonAsync<Response<Category?>>($"v1/categories/{request.Id}")
                ?? new Response<Category?>(null,400,"Não foi possível obter a categoria");
        }

        public async Task<Response<Category?>> UpdateAsync(UpdateCategoryRequest request)
        {
            var result = await _client.PutAsJsonAsync($"v1/categories/{request.Id}", request);

            return await result.Content.ReadFromJsonAsync<Response<Category?>>()
                ?? new Response<Category?>(null, 400, "Falha ao atualizar a categoria");
        }
    }
}
