using Azure.Core;
using System.Net.Http.Headers;

namespace ECommerceWebsite
{
    public class TokenManagerMiddleware
    {
        private readonly RequestDelegate _delegate;
        public TokenManagerMiddleware(RequestDelegate requestDelegate)
        {
            _delegate = requestDelegate;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            var token =string.Empty;
            httpContext.Request.Cookies.TryGetValue("token", out token);
            if (token != null) 
            {
                httpContext.Request.Headers.Add("Authorization", "Bearer " + token);
            }
            await _delegate(httpContext);
        }
    }
}
