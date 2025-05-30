using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Microsoft.EntityFrameworkCore;
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
