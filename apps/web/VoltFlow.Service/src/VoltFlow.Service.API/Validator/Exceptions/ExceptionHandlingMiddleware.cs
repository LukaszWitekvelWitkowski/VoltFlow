using FluentValidation;

namespace VoltFlow.Service.API.Validator.Exceptions
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var response = new
                {
                    _IsSuccess = false,
                    _Message = "Błąd walidacji",
                    _Errors = ex.Errors.Select(x => new { x.PropertyName, x.ErrorMessage })
                };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
