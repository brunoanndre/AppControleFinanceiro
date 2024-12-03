using System.Drawing;
using System.Text.Json.Serialization;

namespace Dima.Core.Responses
{
    public class PagedResponse<TData> : Response<TData>
    {
        public int CurrentPage { get; set; }
        public int TotalPage => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public int PageSize { get; set; } = Configuration.DefaultPageSize;
        public int TotalCount { get; set; }

        [JsonConstructor]
        public PagedResponse(TData? data, int totalCount, int currentPage = 1, int pageSize = Configuration.DefaultPageSize) :base(data)
        {
            PageSize = Configuration.DefaultPageSize;
            TotalCount = totalCount;
            CurrentPage = currentPage;
            Data = data;
            PageSize = pageSize;
        }

        public PagedResponse(TData? data, int code = Configuration.DefaultStatusCode, string? message = null) :base(data,code,message)
        {
        }
    }
}
