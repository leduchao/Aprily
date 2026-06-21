using Aprily.Backend.Common.Extensions;
using Aprily.Backend.Features.Chat;
using Aprily.Backend.Features.Chat.Hubs;
using Aprily.Backend.Features.Friends;
using Aprily.Backend.Features.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddServiceCollection(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors(ServiceCollectionExtension.CorsPolicyName);

var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "Uploads");
Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(uploadsPath),
    RequestPath = "/api/uploads"
});

app.UseAuthentication();
app.UseAuthorization();

app.MapUsersEndpoints();
app.MapFriendsEndpoints();

app.MapChatEndpoints();
app.MapHub<ChatHub>(ChatHub.HubPath);

app.MapGet("/", () => Results.Ok(new { greetingMessage = "Welcome to Aprily" }))
    .WithName("Greeting");

app.Run();
