using ChartProject.Api.Models.Dtos;

namespace ChartProject.Api.Middleware
{
    public class ConnectionInfoMiddleware
    {
        private readonly RequestDelegate _next;

        public ConnectionInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var connectionInfo = new ConnectionInfoDto
            {
                ServerName = "THEKUMRAL\\DEVELOPEREDITION",
                DatabaseName = "ChartsDb",
                DataSource = "ChartData"
            };

            context.Items["ConnectionInfo"] = connectionInfo;

            await _next(context);
        }
    }
}
