using FluentValidation;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Moongazing.Kernel.Application.Pipelines.Authorization;
using Moongazing.Kernel.Application.Pipelines.Caching;
using Moongazing.Kernel.Application.Pipelines.Logging;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.Application.Pipelines.Transaction;
using Moongazing.Kernel.Application.Pipelines.Validation;
using Moongazing.Kernel.Application.Rules;
using Moongazing.Kernel.CrossCuttingConcerns.Logging.Serilog;
using Moongazing.Kernel.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using Moongazing.Kernel.CrossCuttingConcerns.Logging.Serilog.Logger;
using Moongazing.Kernel.Localization;
using Moongazing.Kernel.Mailing;
using Moongazing.Kernel.Mailing.MailKitImplementations;
using Moongazing.Kernel.Security.EmailAuthenticator;
using Moongazing.Kernel.Security.JWT;
using Moongazing.Kernel.Security.OtpAuthenticator;
using Moongazing.Kernel.Security.OtpAuthenticator.OtpNet;
using System.Reflection;

namespace Moongazing.Clinicly.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
                                                            MailSettings mailSettings,
                                                            MsSqlConfiguration loggerConfig
                                                            )
    {
        services.AddMapster();
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            configuration.AddOpenBehavior(typeof(AuthorizationBehavior<,>));
            configuration.AddOpenBehavior(typeof(RateLimiterBehavior<,>));
            configuration.AddOpenBehavior(typeof(CachingBehavior<,>));
            configuration.AddOpenBehavior(typeof(CacheRemovingBehavior<,>));
            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));
            configuration.AddOpenBehavior(typeof(RequestValidationBehavior<,>));
            configuration.AddOpenBehavior(typeof(TransactionScopeBehavior<,>));
        });
        services.AddSubClassesOfType(Assembly.GetExecutingAssembly(), typeof(BaseBusinessRules));

        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddSingleton<ILogger, LoggerServiceBase>(_ => new MsSqlLogger(loggerConfig));
        services.AddSingleton<IMailService, MailKitMailService>(_ => new MailKitMailService(mailSettings));
        services.AddScoped<ITokenHelper, JwtHelper>();
        services.AddScoped<IEmailAuthenticatorHelper, EmailAuthenticatorHelper>();
        services.AddScoped<IOtpAuthenticatorHelper, OtpNetOtpAuthenticatorHelper>();







        services.AddYamlResourceLocalization();


        return services;

    }
    public static IServiceCollection AddSubClassesOfType(this IServiceCollection services,
                                                         Assembly assembly,
                                                         Type type,
                                                         Func<IServiceCollection, Type,
                                                         IServiceCollection>? addWithLifeCycle = null)
    {
        var types = assembly.GetTypes()
                            .Where(t => t.IsSubclassOf(type) && type != t)
                            .ToList();

        foreach (var item in types)
            if (addWithLifeCycle == null)
                services.AddScoped(item);
            else
                addWithLifeCycle(services, type);
        return services;
    }
  
}