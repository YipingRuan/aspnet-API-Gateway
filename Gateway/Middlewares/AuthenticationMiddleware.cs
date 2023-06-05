﻿namespace Gateway.Middlewares
{
    // Convert JWT/API-key to request context in header
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            ProcessAuthentication(context);
            await _next.Invoke(context);
        }

        public void ProcessAuthentication(HttpContext context)
        {
            // Get API key and fill user information
            string v = context.Request.Headers["test"].FirstOrDefault() + "";
            context.Request.Headers.Add("test_added", v + " added");
        }
    }
}