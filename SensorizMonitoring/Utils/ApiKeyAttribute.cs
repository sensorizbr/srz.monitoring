using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class ApiKeyAttribute : TypeFilterAttribute
{
    public ApiKeyAttribute() : base(typeof(ApiKeyFilter)) { }

    private class ApiKeyFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Get the API key from the request header
            var apiKey = context.HttpContext.Request.Headers["X-API-KEY"];

            // Validate the API key (e.g., check if it exists in a database or a configuration file)
            if (!IsValidApiKey(apiKey))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            await next();
        }

        private bool IsValidApiKey(string apiKey)
        {
            string validApiKey = "4a46872c-eaa9-4759-9655-e7fd35592e39";
            return apiKey != null && apiKey.Equals(validApiKey, StringComparison.OrdinalIgnoreCase);
        }
    }
}