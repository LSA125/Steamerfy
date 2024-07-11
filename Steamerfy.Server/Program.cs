using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Steamerfy.Server.HubsAndSockets;
using Steamerfy.Server.Services;
using Steamerfy.Server.ExternalApiHandlers;
using Steamerfy.Server.Factory;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder
            .WithOrigins("https://victorious-stone-026d5800f.5.azurestaticapps.net")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.PropertyNamingPolicy = null;
});

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