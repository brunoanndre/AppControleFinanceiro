using System.Text.Json.Serialization;

namespace Dima.Core.Responses
{
    public class Response<TData>
    {

        private readonly int Code;
        public TData? Data { get; set; }
        public string? Message { get; set; }

        [JsonConstructor]
        public Response()
            => Code = Configuration.DefaultStatusCode;

        public Response(TData? data, int code = Configuration.DefaultStatusCode, string? message = null)
        {
            Data = data;
            Message = message;
            Code = code;
        }

        [JsonIgnore]
        public bool IsSuccess => Code is >= 200 and <= 299;
        

    }
}
