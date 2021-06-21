using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace ForExercising.CustomMiddlewares
{
    public partial class Startup
    {
        public class CustomMiddleware
        {
            private readonly RequestDelegate next;

            public CustomMiddleware(RequestDelegate next)
            {
                this.next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                await context.Response.WriteAsync("Hello");
                await this.next(context);
            }
        }
    }
}
