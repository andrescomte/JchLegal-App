using JchLegal.Domain.Services;

namespace JchLegal.ApplicationApi.Middleware
{
    public class TenantMiddleware : IMiddleware
    {
        private readonly ITenantContext _tenantContext;

        public TenantMiddleware(ITenantContext tenantContext)
        {
            _tenantContext = tenantContext;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/health") ||
                _tenantContext.IsResolved)
            {
                await next(context);
                return;
            }

            var tenantCode = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();

            if (string.IsNullOrEmpty(tenantCode))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Missing X-Tenant-Id header.");
                return;
            }

            // TODO: resolver tenant desde LexDbContext cuando esté disponible
            // Por ahora se acepta cualquier tenant con id=1 para desarrollo
            _tenantContext.SetTenant(1, tenantCode);

            await next(context);
        }
    }
}
