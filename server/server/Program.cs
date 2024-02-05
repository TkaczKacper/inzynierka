using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using server.Authorization;
using server.Helpers;
using server.Services;
using server.Utilities;

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
    services.Configure<StravaSettings>(builder.Configuration.GetSection("Strava"));

    // configure dependency injection for app services
    services.AddScoped<IJwtUtils, JwtUtils>();
    services.AddScoped<IUserService, UserService>();
    services.AddScoped<IPasswordHasher, PasswordHasher>();
    services.AddScoped<IStravaService, ProfileService>();
    services.AddScoped<IActivityService, ActivityService>();
    services.AddScoped<IStravaApiService, StravaApiService>();
    services.AddScoped<IProcessActivityService, ProcessActivityService>();
    services.AddScoped<IHelperService, HelperService>();

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


//app.UseHttpsRedirection();


app.MapControllers();

app.Run();
