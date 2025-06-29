using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moongazing.Clinicly.Application.Repositories;
using Moongazing.Clinicly.Persistence.Context;
using Moongazing.Clinicly.Persistence.Repositories;

namespace Moongazing.Clinicly.Persistence;

public static class PersistenceServiceRegistrations
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BaseDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("CliniclyLocal"),
                sqlOptions =>
                {
                    sqlOptions.CommandTimeout(360);
                }));
        services.AddScoped<IUserRepository, UserRepository>();

        return services;

    }
}