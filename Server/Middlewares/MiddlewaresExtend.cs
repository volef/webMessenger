using Microsoft.AspNetCore.Builder;

namespace Server.Middlewares
{
    public static class MiddlewaresExtend
    {
        public static IApplicationBuilder UseSocketMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<SocketMiddleware>();
            return app;
        }
    }
}