using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApi;
using WebApi.Data;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// KeyVault configuration
var keyVaultUri = new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/");

if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        keyVaultUri,
        new DefaultAzureCredential(),
        new AzureKeyVaultConfigurationOptions()
        {
            Manager = new CustomSecretManager("Ventixe"),
            ReloadInterval = TimeSpan.FromMinutes(5)
        }
    );
}

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("Admin", policy => policy.RequireRole("Admin"))
    .AddPolicy("User", policy => policy.RequireRole("User"));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<BookingService>();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlDbConnection")));

builder.Services.AddGrpcClient<EventProto.EventProtoClient>(o =>
{
    o.Address = new Uri(builder.Configuration["Grpc:EventService"]!);
});

var app = builder.Build();

app.MapOpenApi();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
