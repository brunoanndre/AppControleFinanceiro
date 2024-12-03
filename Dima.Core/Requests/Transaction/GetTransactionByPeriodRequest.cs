
namespace Dima.Core.Requests.Transaction 
{
    public class GetTransactionByPeriodRequest : PagedRequest
    {
        public DateTime? InitialDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
