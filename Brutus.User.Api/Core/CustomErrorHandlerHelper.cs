using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using Brutus.Core;
using Brutus.User.Exceptions;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Brutus.User.Api.Core
{
    public static class CustomErrorHandlerHelper
    {
        public static void HandleException(this IApplicationBuilder app, bool includeDetails)
        {
            app.Use(async (httpContext, next) =>
            {
                var exceptionDetails = httpContext.Features.Get<IExceptionHandlerFeature>();
                var ex = exceptionDetails?.Error;

                if (ex != null)
                {
                    httpContext.Response.ContentType = "application/problem+json";
                    ProblemDetails problem;

                    if (ex.GetType() == typeof(RequestFaultException) && ((RequestFaultException) ex).Fault.Exceptions[0] != null)
                    {
                        httpContext.Response.StatusCode = 400;
                        string title = string.Empty;
                        List<string> errors = new List<string>();
                        var faultEx = ((RequestFaultException) ex).Fault.Exceptions[0];
                        switch (faultEx.ExceptionType.Split(".").Last())
                        {
                            case nameof(DomainException):
                                title = $"One or more domain business errors occured";
                                errors.Add(faultEx.Message);
                                break;
                            case nameof(DataValidationException):
                                title = $"One or more data errors occured";
                                errors.Add(faultEx.Message);
                                break;
                        }
                        
                        problem = new ProblemDetails
                        {
                            Status = 409,
                            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.8",
                            Title = title
                        };
                        problem.Extensions["errors"] = errors;
                    }
                    else
                    {
                        httpContext.Response.StatusCode = 500;
                        problem = new ProblemDetails
                        {
                            Status = 500,
                            Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                            Title = includeDetails ? "An error occured: " + ex.Message : "An error occured",
                            Detail = includeDetails ? ex.ToString() : null
                        };
                    }

                    var traceId = Activity.Current?.Id ?? httpContext?.TraceIdentifier;
                    if (traceId != null)
                    {
                        problem.Extensions["traceId"] = traceId;
                    }

                    var stream = httpContext.Response.Body;
                    await JsonSerializer.SerializeAsync(stream, problem);
                }
            });
        }
    }
}