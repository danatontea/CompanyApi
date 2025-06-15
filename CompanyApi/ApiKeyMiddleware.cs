namespace CompanyApi
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip API Key pentru endpoint-uri publice
            if (context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/health") ||
                context.Request.Path.Value == "/")
            {
                await _next(context);
                return;
            }

            // Verifică dacă header-ul X-Api-Key există
            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("API Key is missing");
                return;
            }

            // Verifică dacă API Key-ul este valid
            var validApiKey = _configuration["ApiSettings:ApiKey"];
            if (extractedApiKey != validApiKey)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            // API Key valid - continuă cu request-ul
            await _next(context);
        }
    }
}
