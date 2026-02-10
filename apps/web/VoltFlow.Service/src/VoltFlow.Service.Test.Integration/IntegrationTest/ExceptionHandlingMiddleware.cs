using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;
using VoltFlow.Service.Core.Exceptions;
using VoltFlow.Service.Core.Models.Common;

namespace VoltFlow.Service.Test.Integration.IntegrationTest
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
            catch (NotFoundException ex)
            {
                context.Response.ContentType = "application/json";
                // Zwracamy 200 lub 404 w zależności od tego, jak Twój frontend obsługuje koperty
                context.Response.StatusCode = (int)HttpStatusCode.OK;

                var response = ServiceResponse<object>.Failure(ex.Message, 404);
                await context.Response.WriteAsJsonAsync(response);
            }
        }

        // W Twoim Middleware
        private static Task HandleExceptionAsync(HttpContext context, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.OK; // Status HTTP zostawiamy 200 zgodnie z Twoją architekturą

            // KLUCZOWE: Upewnij się, że statusCode trafia do pola _StatusCode
            var response = ServiceResponse<object>.Failure(message, statusCode);
            return context.Response.WriteAsJsonAsync(response);
        }
    }
}
