using System.Diagnostics;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Core.Models.Validators
{
    public static class ResponseValidator
    {
        public static async Task<ServiceResponse<T>> ExecuteAsync<T>(
            Func<Task<T?>> repoCall,
            string entityName = "Data")
        {
            var sw = Stopwatch.StartNew(); // Startujemy stoper
            try
            {
                var data = await repoCall();
                sw.Stop(); // Zatrzymujemy po wykonaniu

                var response = EnsureSuccessAndData(data, entityName);
                response._ExecutionTimeMs = sw.ElapsedMilliseconds; // Przypisujemy czas

                return response;
            }
            catch (Exception ex)
            {
                sw.Stop();
                var response = ServiceResponse<T>.Failure($"{entityName} error: {ex.Message}", 500, entityName);
                response._ExecutionTimeMs = sw.ElapsedMilliseconds;
                return response;
            }
        }

        // 2. Wersja dla akcji (Commands) - zwraca true/false (np. Delete, Update)
        public static async Task<ServiceResponse<bool>> ExecuteAsync(
            Func<Task> repoCall,
            string entityName = "Operation")
        {
            var sw = Stopwatch.StartNew(); // Startujemy pomiar
            try
            {
                await repoCall();
                sw.Stop(); // Zatrzymujemy po sukcesie

                var response = ServiceResponse<bool>.Success(true, entityName);
                response._ExecutionTimeMs = sw.ElapsedMilliseconds;
                return response;
            }
            catch (Exception ex)
            {
                sw.Stop(); // Zatrzymujemy mimo błędu
                var response = ServiceResponse<bool>.Failure($"{entityName} failed: {ex.Message}", 500, entityName);
                response._ExecutionTimeMs = sw.ElapsedMilliseconds;
                return response;
            }
        }

        public static ServiceResponse<T> EnsureSuccessAndData<T>(T? data, string entityName = "Data")
        {
            if (data == null) return ServiceResponse<T>.Failure($"{entityName} not found.", 404);
            return ServiceResponse<T>.Success(data, entityName);
        }
    }
}
