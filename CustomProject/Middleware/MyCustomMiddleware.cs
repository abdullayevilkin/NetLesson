using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomProject.Middleware
{
    public class MyCustomMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            await context.Response.WriteAsync("hello from");             
        }
    }

    public static class CustomMiddlewareClass
    {
        public static IApplicationBuilder UseMyCustomMiddleware(this IApplicationBuilder ap)
        {
            return ap .UseMiddleware<MyCustomMiddleware>();
        }
    }
}