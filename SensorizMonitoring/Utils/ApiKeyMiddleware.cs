using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class ApiKeyMiddleware : IMiddleware
{
    private const string ApiKeyHeader = "X-Api-Key";
    private const string ExpectedApiKey = "4a46872c-eaa9-4759-9655-e7fd35592e39"; // Substitua pela sua chave real

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var apiKey) || apiKey != ExpectedApiKey)
        {
            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Chave da API inválida");
            return;
        }

        await next(context);
    }
}