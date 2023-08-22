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
        // Kiểm tra nếu người dùng chưa đăng nhập và đang truy cập vào một trang khác trang đăng nhập
        if (!context.User.Identity.IsAuthenticated && !context.Request.Path.Equals("/Home/Index") && !_isLoginPageDisplayed)
        {
            // Chuyển hướng sang trang đăng nhập
            context.Response.Redirect("/Home/Index");
            _isLoginPageDisplayed = true; // Đánh dấu là trang đăng nhập đã được hiển thị
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
