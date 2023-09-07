using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using server.Authorization;
using server.Helpers;
using server.Services;

var builder = WebApplication.CreateBuilder(args);

{
    var services = builder.Services;
    var env = builder.Environment;

    // Add services to the container.
    services.AddControllers();
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen();


    services.AddDbContext<DataContext>(
        o => o.UseNpgsql(builder.Configuration.GetConnectionString("Database"))
    );

    services.AddCors();
    services.AddControllers()
        .AddJsonOptions(x => x.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull);

    services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    // configure dependency injection for app services
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IStravaService, StravaService>();
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

{
    app.UseCors(x => x
        .SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials());

    app.UseMiddleware<ErrorHandlerMiddleware>();

    app.UseMiddleware<JwtMiddleware>();
}


app.UseHttpsRedirection();


app.MapControllers();

app.MapGet("/", () => "test");


app.Run();
