var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(option => option.AddServerHeader = false); // Disable server header
builder.Services.AddHealthChecks();

builder.AutoConfigReverseProxy("YarpConfigs");

var app = builder.Build();
app.UseHealthChecks("/health");
app.MapReverseProxy();
app.Run();