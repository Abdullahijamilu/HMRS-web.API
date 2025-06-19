namespace HMRS_web.API.DTO
{
    public class ResponseModel<T>
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
    }
}
