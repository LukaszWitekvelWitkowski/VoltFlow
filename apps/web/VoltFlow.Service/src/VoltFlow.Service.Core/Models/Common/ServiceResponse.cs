using System.Text.Json.Serialization;

namespace VoltFlow.Service.Core.Models.Common
{
    public class ServiceResponse<T>
    {
        public T? _Data { get; set; }
        public bool _IsSuccess { get; set; } = true;
        public string _Message { get; set; } = string.Empty;
        public string _EntityName { get; set; } = string.Empty;

        [JsonInclude]
        public int _StatusCode { get; private set; }
        public long _ExecutionTimeMs { get; set; }

        public static ServiceResponse<T> Result(T? data,  string entityName = "") => new() { _Data = data, _IsSuccess = true, _StatusCode = 200, _EntityName = entityName };
        public static ServiceResponse<T> Success(T data, string entityName = "") => new() { _Data = data, _IsSuccess = true, _EntityName = entityName };
        public static ServiceResponse<T> Failure(string message, int code = 400, string entityName = "") => new() { _Message = message, _IsSuccess = false, _StatusCode = code, _EntityName = entityName };
    }
}
