using Carter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Moongazing.Clinicly.Api;
using Moongazing.Clinicly.Application;
using Moongazing.Clinicly.Persistence;
using Moongazing.Kernel.Application.Pipelines.RateLimiting;
using Moongazing.Kernel.CrossCuttingConcerns.Exceptions.Extensions;
using Moongazing.Kernel.CrossCuttingConcerns.Logging.Serilog.ConfigurationModels;
using Moongazing.Kernel.Localization.Middlewares;
using Moongazing.Kernel.Mailing;
using Moongazing.Kernel.Persistence.MigrationApplier;
using Polly;
using StackExchange.Redis;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Text.Json.Serialization;
using TokenOptions = Moongazing.Kernel.Security.JWT.TokenOptions;


var builder = WebApplication.CreateBuilder(args);


#region Services
builder.Services.Configure<RateLimitSettings>(
    builder.Configuration.GetSection("RateLimitSettings"));
builder.Services.AddMemoryCache();
builder.Services.AddApplicationServices(
                     builder.Configuration.GetSection("MailSettings").Get<MailSettings>()!,
                     builder.Configuration.GetSection("SeriLogConfigurations:PostgreSqlConfiguration").Get<PostgreSqlConfiguration>()!);

builder.Services.AddHttpContextAccessor();

builder.Services.AddPersistenceServices(builder.Configuration);

builder.Services.AddCarter();

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);



#endregion
#region TokenOptions
const string tokenOptionsConfigurationSection = "TokenOptions";
TokenOptions tokenOptions =
    builder.Configuration.GetSection(tokenOptionsConfigurationSection).Get<TokenOptions>()
    ?? throw new InvalidOperationException($"\"{tokenOptionsConfigurationSection}\" Section Cannot Found In Configuration");
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = tokenOptions.Issuer,
            ValidAudience = tokenOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = Moongazing.Kernel.Security.Encryption.SecurityKeyHelper.CreateSecurityKey(tokenOptions.SecurityKey)
        };

    });

#endregion
#region Cors
builder.Services.AddCors(opt =>
    opt.AddDefaultPolicy(p =>
    {
        _ = p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    })
);
#endregion
#region Redis

var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
var redisRetryPolicy = Policy
    .Handle<RedisConnectionException>()
    .WaitAndRetry(
    [
        TimeSpan.FromSeconds(5),
        TimeSpan.FromSeconds(10),
        TimeSpan.FromSeconds(15)
    ]);

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(redisConnectionString!, true);
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
});
#endregion
#region Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Clinicy.Api",
        Version = "v1"
    });
    opt.AddSecurityDefinition(
        name: "Bearer",
        securityScheme: new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer YOUR_TOKEN\". \r\n\r\n"
                + "`Enter your token in the text input below.`"
        }
    );
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            Array.Empty<string>()
        }
    });
});
#endregion
var app = builder.Build();
#region Environment

if (app.Environment.IsDevelopment())
{

    app.ConfigureCustomExceptionMiddleware();

    _ = app.UseSwagger();

    _ = app.UseSwaggerUI(opt =>
    {
        opt.DocExpansion(DocExpansion.None);
    });
}
else
{
    app.ConfigureCustomExceptionMiddleware();

    _ = app.UseSwagger();

    _ = app.UseSwaggerUI(opt =>
    {
        opt.DocExpansion(DocExpansion.None);
    });
}
#endregion

#region WebApiConfiguration
const string webApiConfigurationSection = "WebAPIConfiguration";
WebApiConfiguration webApiConfiguration =
    app.Configuration.GetSection(webApiConfigurationSection).Get<WebApiConfiguration>()
    ?? throw new InvalidOperationException($"\"{webApiConfigurationSection}\" Section Cannot Found In Configuration");
#endregion

app.UseCors(opt => opt.WithOrigins(webApiConfiguration.AllowedOrigins).AllowAnyHeader().AllowAnyMethod());

app.UseDbMigrationApplier();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.MapCarter();     

app.UseResponseLocalization();

app.Run();