namespace Api.MiddleWare;

/// <summary>
/// Middleware to add security headers to all HTTP responses
/// </summary>
public class SecurityHeadersMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IHostEnvironment _environment;

    public SecurityHeadersMiddleware(RequestDelegate next, IHostEnvironment environment)
    {
        _next = next;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add X-Content-Type-Options: nosniff
        // Prevents MIME type sniffing
        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

        // Add X-Frame-Options: DENY
        // Prevents clickjacking attacks
        context.Response.Headers.Append("X-Frame-Options", "DENY");

        // Add X-XSS-Protection: 1; mode=block
        // Enables XSS protection (legacy browsers)
        context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

        // Add Referrer-Policy: strict-origin-when-cross-origin
        // Controls how much referrer information is sent
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

        // Add Content-Security-Policy
        // Defines approved sources of content for the website
        if (_environment.IsDevelopment())
        {
            // Relaxed CSP for development
            context.Response.Headers.Append("Content-Security-Policy",
                "default-src 'self'; " +
                "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                "style-src 'self' 'unsafe-inline'; " +
                "img-src 'self' data: https:; " +
                "font-src 'self'; " +
                "connect-src 'self'; " +
                "frame-ancestors 'none';");
        }
        else
        {
            // Strict CSP for production
            context.Response.Headers.Append("Content-Security-Policy",
                "default-src 'self'; " +
                "script-src 'self'; " +
                "style-src 'self'; " +
                "img-src 'self' data:; " +
                "font-src 'self'; " +
                "connect-src 'self'; " +
                "frame-ancestors 'none'; " +
                "base-uri 'self'; " +
                "form-action 'self';");
        }

        // Add Permissions-Policy (formerly Feature-Policy)
        // Controls which browser features can be used
        context.Response.Headers.Append("Permissions-Policy",
            "geolocation=(), " +
            "microphone=(), " +
            "camera=(), " +
            "fullscreen=(self), " +
            "payment=(self)");

        // Add Strict-Transport-Security (HTTPS only)
        // Only add this header when the request is over HTTPS
        if (context.Request.IsHttps)
        {
            // max-age=31536000 = 1 year
            // includeSubDomains applies HSTS to all subdomains
            context.Response.Headers.Append("Strict-Transport-Security",
                "max-age=31536000; includeSubDomains; preload");
        }

        await _next(context);
    }
}

/// <summary>
/// Extension method to register the SecurityHeadersMiddleware
/// </summary>
public static class SecurityHeadersMiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SecurityHeadersMiddleware>();
    }
}
