using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Galaxy.Auth.Presentation.ViewModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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