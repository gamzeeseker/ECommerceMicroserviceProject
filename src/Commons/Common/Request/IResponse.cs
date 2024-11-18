namespace Common.Request
{
    public interface IResponse<T> where T : new()
    {
        bool IsSuccess { get; set; }
        string Message { get; set; }
        T Data { get; set; }
    }
    public class Response<T> : IResponse<T> where T : new()
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static Response<T> Success(T data)
        {
            return new Response<T>() { Data = data, IsSuccess = true };
        }
        public static Response<T> Fail(T data)
        {
            return new Response<T>() { Data = data, IsSuccess = false };
        }

        public static Response<T> Fail(T data, string message)
        {
            return new Response<T>() { Data = data, IsSuccess = false, Message = message };
        }
    }
}
