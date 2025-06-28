using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moongazing.Clinicly.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moongazing.Clinicly.Persistence.Context;

public class BaseDbContext: DbContext
{
    protected IHttpContextAccessor HttpContextAccessor;
    protected IConfiguration Configuration { get; set; }
    public virtual DbSet<UserEntity> User { get; set; }
    public virtual DbSet<OperationClaimEntity> OperationClaim { get; set; }
    public virtual DbSet<UserOperationClaimEntity> UserOperationClaim { get; set; }
    public virtual DbSet<RefreshTokenEntity> RefreshToken { get; set; }
    public virtual DbSet<EmailAuthenticatorEntity> EmailAuthenticator { get; set; }
    public virtual DbSet<OtpAuthenticatorEntity> OtpAuthenticator { get; set; }


    public BaseDbContext(DbContextOptions<BaseDbContext> options,
                         IConfiguration configuration,
                         IHttpContextAccessor httpContextAccessor) : base(options)
    {
        HttpContextAccessor = httpContextAccessor;
        Configuration = configuration;
    }
}
