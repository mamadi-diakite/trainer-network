using Microsoft.Extensions.DependencyInjection;
using TrainerNetwork.Application.Auth;

namespace TrainerNetwork.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();

        return services;
    }
}
