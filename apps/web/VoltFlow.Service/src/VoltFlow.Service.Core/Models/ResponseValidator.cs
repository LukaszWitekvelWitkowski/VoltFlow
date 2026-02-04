using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VoltFlow.Service.Core.Models
{
    public static class ResponseValidator
    {
        public static ServiceResponse<T> EnsureSuccessAndData<T>(ServiceResponse<T> result, string entityName = "Data")
        {
            // 1.We check whether the repository call itself was successful(e.g.no SQL errors)
            if (!result._IsSuccess)
            {
                if (result._Errors == null || result._Errors.Count == 0)
                {
                    return ServiceResponse<T>.Failure($"{entityName} operation failed: " + result._Message, result._StatusCode);
                }
                return ServiceResponse<T>.Failure($"{entityName} is Error: " + string.Join(", ", result._Errors), result._StatusCode);
            }

            // 2. We check whether the data exists at all
            if (result._Data == null)
            {
                return ServiceResponse<T>.Failure($"{entityName} not found.", 404);
            }

            // 3. If everything is OK, we return the original result
            return result;
        }
    }
}
