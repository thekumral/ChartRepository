using ChartProject.Api.Configurations;
using ChartProject.Api.Data;
using ChartProject.Api.Middleware;
using ChartProject.Api.Models.Dtos;
using ChartProject.Api.Repositories;
using ChartProject.Api.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{
    var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
    var connectionInfo = httpContextAccessor.HttpContext?.Items["ConnectionInfo"] as ConnectionInfoDto;

    if (connectionInfo != null)
    {
        var connectionString = $"Server={connectionInfo.ServerName};Database={connectionInfo.DatabaseName};Integrated Security=True;TrustServerCertificate=True;";
        options.UseSqlServer(connectionString);
    }
    else
    {
        throw new InvalidOperationException("Connection information is not provided.");
    }
});

builder.Services.AddScoped<IChartRepository, ChartRepository>();
builder.Services.AddScoped<IChartService, ChartService>();
builder.Services.AddAutoMapper(typeof(ChartMappingProfile));
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseMiddleware<ConnectionInfoMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();
