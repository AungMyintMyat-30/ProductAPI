using Newtonsoft.Json;
using ProductCore.Models;
using System.Net;

namespace ProductAPI.Middleware
{
    public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError("Runtime Error: {message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            DefaultResponseModel model = new()
            {
                Success = false,
                Meta = null,
                Code = StatusCodes.Status500InternalServerError,
                Data = "",
                Error = ex.Message
            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(model));
        }
    }
}
