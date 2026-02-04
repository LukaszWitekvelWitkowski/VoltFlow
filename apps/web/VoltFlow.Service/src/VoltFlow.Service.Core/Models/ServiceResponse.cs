namespace VoltFlow.Service.Core.Models
{
    public class ServiceResponse<T>
    {
        public T? _Data { get; set; }
        public bool _IsSuccess { get; set; } = true;
        public string _Message { get; set; } = string.Empty;
        public int _StatusCode { get; private set; }
        public List<string>? _Errors { get; set; } 

        public static ServiceResponse<T> Result(T? data) => new() { _Data = data, _IsSuccess = true, _StatusCode = 200 };
        public static ServiceResponse<T> Success(T data) => new() { _Data = data, _IsSuccess = true };
        public static ServiceResponse<T> Failure(string message, int code = 400) => new() { _Message = message, _IsSuccess = false, _StatusCode = code };
    }
}
