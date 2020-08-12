using Microsoft.Extensions.DependencyInjection;

namespace Server.Services
{
    public static class ServicesExtend
    {
        public static void AddMessageRepository(this IServiceCollection services)
        {
            services.AddScoped<MessageRepository>();
        }

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<SocketRepository>();
            services.AddSingleton<SocketHandler>();


            return services;
        }
    }
}