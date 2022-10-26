using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SpicyCatsBlogAPI.Data;
using SpicyCatsBlogAPI.Data.FileManager;
using SpicyCatsBlogAPI.Data.Repository;
using SpicyCatsBlogAPI.Models.Auth;
using SpicyCatsBlogAPI.Services.UserService;
using SpicyCatsBlogAPI.Utils.ActionFilters.Validation;
using Swashbuckle.AspNetCore.Filters;
using System.Net.Mime;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlite(
        builder.Configuration.GetSection("ConnectionStrings:DefaultConnection").Value
    );
});
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddControllers().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var result = new ValidationFailedResult(context.ModelState);
        // TODO: add `using System.Net.Mime;` to resolve MediaTypeNames
        result.ContentTypes.Add(MediaTypeNames.Application.Json);
        return result;
    };
});
builder.Services.AddResponseCaching();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IFileManager, FileManager>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        }
    );
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            // TODO add URL to frontend
            builder.WithOrigins(
                "https://spicycatgames.github.io", 
                "https://spicycat.tech",
                "https://www.spicycat.tech",
                "https://blog.spicycat.tech",
                "http://localhost:3000", 
                "http://localhost")
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseCors();

app.UseResponseCaching();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

try
{
    var scope = app.Services.CreateScope();
    var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (ctx.Database.GetPendingMigrations().Any())
    {
        ctx.Database.Migrate();
    }
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
}

app.Run();

// authorization header to requests on client side
// https://jwt.io/