using Microsoft.Extensions.DependencyInjection;
using TrainerNetwork.Application.Auth;
using TrainerNetwork.Application.Security;
using TrainerNetwork.Application.Users;

namespace TrainerNetwork.Application.Tests;

public sealed class ApplicationDependencyInjectionTests
{
    [Fact]
    public void Registers_the_auth_service()
    {
        var services = new ServiceCollection()
            .AddApplication()
            .AddSingleton<IUserRepository, InMemoryUserRepository>()
            .AddSingleton<IPasswordHasher, FakePasswordHasher>()
            .AddSingleton<IAccessTokenIssuer, FakeTokenIssuer>();

        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetService<AuthService>());
    }
}
