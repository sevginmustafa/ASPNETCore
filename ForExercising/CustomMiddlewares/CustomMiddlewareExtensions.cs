using Microsoft.AspNetCore.Builder;
using static ForExercising.CustomMiddlewares.Startup;

namespace ForExercising.CustomMiddlewares
{
    public static class CustomMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustom(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomMiddleware>();
        }
    }
}
