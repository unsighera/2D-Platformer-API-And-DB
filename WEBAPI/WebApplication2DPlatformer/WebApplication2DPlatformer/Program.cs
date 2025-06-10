using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApplication2DPlatformer.DatabaseContext;
using WebApplication2DPlatformer.Interfaces;
using WebApplication2DPlatformer.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "2D Platformer API",
        Version = "v1",
        Description = "API for 2D Platformer Game"
    });
});


builder.Services.AddDbContext<dbcontext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TestDbString")));

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddSignalR();

var app = builder.Build();

app.MapHub<ChatHub>("/chatHub");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "2D Platformer API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();