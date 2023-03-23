using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace SE1611_Group1_Project.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ViewDataMiddleware
    {
        private readonly RequestDelegate _next;

        public ViewDataMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Items["Count"] = httpContext.Session.GetInt32("Count");
            httpContext.Items["OrderDetailList"] = httpContext.Session.GetString("OrderDetailList");
            await _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ViewDataMiddlewareExtensions
    {
        public static IApplicationBuilder UseViewDataMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ViewDataMiddleware>();
        }
    }
}
