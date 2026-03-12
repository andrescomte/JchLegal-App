using FluentValidation;
using FluentValidation.AspNetCore;
using JchLegal.ApplicationApi.Middleware;
using JchLegal.Domain.Repository;
using JchLegal.Domain.Services;
using JchLegal.Domain.SeedWork;
using JchLegal.Infrastructure.Context;
using JchLegal.Infrastructure.Repository;
using JchLegal.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:8082")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "JchLegal API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa tu JWT token."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var services = builder.Services;

services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JWT configuration is missing from appsettings.");

services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
        {
            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var tenantIdClaim = jwt.Claims.FirstOrDefault(c => c.Type == "tenant_id")?.Value;
            if (!int.TryParse(tenantIdClaim, out var tenantId))
                return [];
            var baseKey = Encoding.UTF8.GetBytes(jwtSettings.Key);
            using var hmac = new System.Security.Cryptography.HMACSHA256(baseKey);
            var derivedKey = hmac.ComputeHash(Encoding.UTF8.GetBytes(tenantId.ToString()));
            return [new SymmetricSecurityKey(derivedKey)];
        }
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            var tenantCtx = context.HttpContext.RequestServices.GetRequiredService<ITenantContext>();
            var tokenTenantIdClaim = context.Principal?.FindFirst("tenant_id")?.Value;
            if (!int.TryParse(tokenTenantIdClaim, out var tokenTenantId) || tokenTenantId != tenantCtx.TenantId)
                context.Fail("El token no pertenece al tenant actual.");
            return Task.CompletedTask;
        }
    };
});

services.AddDbContext<JchLegalDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("MainConnectionString")));

services.AddAuthorization();
services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining(typeof(Program)));
services.AddHttpContextAccessor();

services.AddScoped<ITenantContext, TenantContext>();
services.AddScoped<IFileStorageService, LocalFileStorageService>();
services.AddScoped<IAttachmentRepository, AttachmentRepository>();
services.AddScoped<TenantMiddleware>();
services.AddScoped<ExceptionMiddleware>();

services.AddScoped<IClientRepository, ClientRepository>();
services.AddScoped<ICasePartRepository, CasePartRepository>();
services.AddScoped<ICaseRepository, CaseRepository>();
services.AddScoped<IBitacoraRepository, BitacoraRepository>();
services.AddScoped<IHearingRepository, HearingRepository>();
services.AddScoped<IFeeRepository, FeeRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("FrontendPolicy");

var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "uploads");
Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads"
});

app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<TenantMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
