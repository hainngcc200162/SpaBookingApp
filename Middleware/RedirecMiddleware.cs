using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

public class RedirectMiddleware
{
    private readonly RequestDelegate _next;
    private static bool _isLoginPageDisplayed = false;

    public RedirectMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        if (!context.User.Identity.IsAuthenticated && !context.Request.Path.Equals("/Home/Index") && !_isLoginPageDisplayed)
        {
            context.Response.Redirect("/Home/Index");
            _isLoginPageDisplayed = true;
            return;
        }

        await _next(context);
    }
}

public static class RedirectToLoginMiddlewareExtensions
{
    public static IApplicationBuilder UseRedirectToLoginMiddleware(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RedirectMiddleware>();
    }
}
