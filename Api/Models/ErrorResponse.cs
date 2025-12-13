namespace Api.Models
{
    public class ErrorResponse
    {
        public string Code { get; set; } = default!;
        public string Message { get; set; } = default!;
        public Dictionary<string, string[]>? Errors { get; set; }
        public string? TraceId { get; set; }

        public ErrorResponse()
        {
        }

        public ErrorResponse(string code, string message, Dictionary<string, string[]>? errors = null, string? traceId = null)
        {
            Code = code;
            Message = message;
            Errors = errors;
            TraceId = traceId;
        }
    }
}
