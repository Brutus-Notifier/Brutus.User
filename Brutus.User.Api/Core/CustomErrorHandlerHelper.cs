using System.Diagnostics;
using System.Text.Json;
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

                    if (ex.GetType() == typeof(RequestFaultException))
                    {
                        httpContext.Response.StatusCode = 400;
                        problem = new ProblemDetails
                        {
                            Status = 400,
                            Title = ((RequestFaultException) ex).Fault.Exceptions[0].Message,
                        };
                    }
                    else
                    {
                        httpContext.Response.StatusCode = 500;
                        problem = new ProblemDetails
                        {
                            Status = 500,
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