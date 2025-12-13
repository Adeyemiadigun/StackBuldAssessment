namespace Application.Common.Models
{
    public record DataResponse<T> {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T? Data { get; set; }
    }
    
}
