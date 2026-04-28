namespace AccessControl.WebApi.Infrastructure.Core
{
    public class ApiResponse<T>
    {
        public int MessageCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
