using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Steamerfy.Server.HubsAndSockets;
using Steamerfy.Server.Services;
using Steamerfy.Server.ExternalApiHandlers;
using Steamerfy.Server.Factory;
using Microsoft.Azure.SignalR;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder
            .WithOrigins(
                "https://steamify.xyz",
                "https://www.steamify.xyz",
                "https://steamerfyserver20251220133732-ewfyana8hjhffjdx.canadacentral-01.azurewebsites.net",
                "https://localhost:4200",
                "http://localhost:4200"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR().AddAzureSignalR(builder.Configuration["Azure:SignalR:ConnectionString"]).AddJsonProtocol(options => {
    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
}); ;

// Add application services.
builder.Services.AddSingleton<ISteamHandler, SteamHandler>();
builder.Services.AddSingleton<IQuestionFactory, QuestionGenerator>();
builder.Services.AddSingleton<GameService>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Apply CORS policy before Authorization middleware
app.UseCors("AllowSpecificOrigins");
app.MapControllers();
app.MapHub<GameHub>("/gameHub");

app.Run();