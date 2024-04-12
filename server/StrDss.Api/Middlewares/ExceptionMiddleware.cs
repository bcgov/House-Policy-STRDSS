using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrDss.Common;
using System.Net;

namespace StrDss.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<StrDssLogger> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (httpContext.Response.HasStarted || httpContext.RequestAborted.IsCancellationRequested)
                    return;

                var guid = Guid.NewGuid();
                await HandleConcurrencyExceptionAsync(httpContext, guid, ex.Message);
            }
            catch (Exception ex)
            {
                if (httpContext.Response.HasStarted || httpContext.RequestAborted.IsCancellationRequested)
                    return;

                var guid = Guid.NewGuid();
                _logger.LogError($"STRDSS Exception {guid}: {ex}");
                await HandleExceptionAsync(httpContext, guid);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Guid guid)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var problem = new ValidationProblemDetails()
            {
                Type = "https://strdss.bc.gov.ca/exception",
                Title = "An unexpected error occurred!",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "The instance value should be used to identify the problem when calling customer support",
                Instance = $"urn:strdss:error:{guid}"
            };

            problem.Extensions.Add("traceId", context.TraceIdentifier);

            await context.Response.WriteJsonAsync(problem, "application/problem+json");
        }

        private async Task HandleConcurrencyExceptionAsync(HttpContext context, Guid guid, string message)
        {
            context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;

            var errors = new Dictionary<string, string[]>
            {
                { "entity", new string[] { message } }
            };

            var problem = new ValidationProblemDetails(errors)
            {
                Type = "https://strdss.bc.gov.ca/model-validation-error",
                Title = "Update conflict detected",
                Status = StatusCodes.Status422UnprocessableEntity,
                Detail = "Please refer to the errors property for additional details",
                Instance = $"urn:strdss:error:{guid}"
            };

            problem.Extensions.Add("traceId", context.TraceIdentifier);

            await context.Response.WriteJsonAsync(problem, "application/problem+json");
        }


    }
}
