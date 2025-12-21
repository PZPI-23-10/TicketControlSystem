using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Ticket_control_system.Data;
using Ticket_control_system.Interfaces;
using Ticket_control_system.Services;
using TicketControlSystem.Data.Models;
using Ticket_control_system.Extensions;
using System.Text.Json.Serialization;
using Ticket_control_system.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ITariffService, TariffService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddHostedService<DeviceStatusChecker>();

builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;       
        options.Password.RequireUppercase = false;     
        options.Password.RequireNonAlphanumeric = false;  
        options.Password.RequiredLength = 1;             
        options.Password.RequiredUniqueChars = 0;         

    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();