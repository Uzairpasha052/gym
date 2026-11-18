using backend.Data;
using backend.Extensions;
using backend.Interfaces;
using backend.Jwt;
using backend.Services;
using backend.Services.Interfaces;
// using Microsoft.Extensions.Options;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        builder.Configuration["Jwt:Key"]!
                    ))
        };
});

//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IJwtService, JwtService>();
//builder.Services.AddScoped<IAttendanceService, AttendanceService>();
//builder.Services.AddScoped<IPlayerService, PlayerService>();
//builder.Services.AddScoped<IReportService, ReportService>();
//builder.Services.AddScoped<ICashService, CashService>();

builder.Services.AddCors(Options =>
{
   Options.AddPolicy("AllowAngular", policy =>
   {
      policy.WithOrigins("http://localhost:4200") // Default Angular Port
            .AllowAnyHeader()
            .AllowAnyMethod();
   });  
});

//Services and Repositories dependencies
builder.Services.AddApplicationDependencies();

//Swagger
builder.Services.AddSwaggerDocumentation();

QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    //app.UseSwaggerUI();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "backend v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


using(var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();
    await SeedData.Initialize(context);
}

app.Run();
