namespace Final_Grp6_PROG3340_UI.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async System.Threading.Tasks.Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            // Redirect to login if unauthorized
            if (context.Response.StatusCode == 401 && !context.Request.Path.StartsWithSegments("/Auth"))
            {
                context.Response.Redirect("/Auth/Login?returnUrl=" + Uri.EscapeDataString(context.Request.Path + context.Request.QueryString));
            }
        }
    }

    public static class AuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
