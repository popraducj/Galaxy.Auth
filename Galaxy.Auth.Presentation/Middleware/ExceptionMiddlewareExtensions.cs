using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Galaxy.Auth.Presentation.Middleware
{
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var error = context.Features.Get<IExceptionHandlerFeature>().Error;
                    var unauthorizedException = error is UnauthorizedAccessException;

                    context.Response.ContentType = "application/json";
                    if (unauthorizedException)
                    {
                        context.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
                        return;
                    }
                    
                    logger.LogCritical($"General error: {error}");
                    
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    await Task.CompletedTask;
                });
            });
        }

    }
}