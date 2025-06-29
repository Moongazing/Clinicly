using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moongazing.Clinicly.Domain.Entities;
using System.Reflection;

namespace Moongazing.Clinicly.Persistence.Context;

public class BaseDbContext: DbContext
{
    protected IHttpContextAccessor HttpContextAccessor;
    protected IConfiguration Configuration { get; set; }
    public virtual DbSet<UserEntity> Users { get; set; }
    public virtual DbSet<OperationClaimEntity> OperationClaims { get; set; }
    public virtual DbSet<UserOperationClaimEntity> UserOperationClaims { get; set; }
    public virtual DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public virtual DbSet<EmailAuthenticatorEntity> EmailAuthenticators { get; set; }
    public virtual DbSet<OtpAuthenticatorEntity> OtpAuthenticators { get; set; }


    public BaseDbContext(DbContextOptions<BaseDbContext> options,
                         IConfiguration configuration,
                         IHttpContextAccessor httpContextAccessor) : base(options)
    {
        HttpContextAccessor = httpContextAccessor;
        Configuration = configuration;
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());


        base.OnModelCreating(modelBuilder);
    }
}
