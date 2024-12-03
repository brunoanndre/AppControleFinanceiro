using System.ComponentModel.DataAnnotations;

namespace Dima.Core.Requests.Category
{
    public class GetCategoryByIdRequest : Request
    {
        public long Id { get; set; }
    }
}
