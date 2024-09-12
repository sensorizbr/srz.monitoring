using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;

    public ApiKeyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if the request is for the Swagger
        if (context.Request.Path.StartsWithSegments("/swagger"))
        {
            // If it's a Swagger request, skip the API key validation
            await _next(context);
            return;
        }

        // Get the API key from the request header
        var apiKey = context.Request.Headers["X-API-KEY"];

        // Validate the API key (e.g., check if it exists in a database or a configuration file)
        if (!IsValidApiKey(apiKey))
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Invalid API key");
            return;
        }

        // If the API key is valid, continue with the request
        await _next(context);
    }

    private bool IsValidApiKey(string apiKey)
    {
        string validApiKey = "4a46872c-eaa9-4759-9655-e7fd35592e39";
        return apiKey != null && apiKey.Equals(validApiKey, StringComparison.OrdinalIgnoreCase);
    }
}