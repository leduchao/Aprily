using Aprily.Backend.Common.Extensions;
using Aprily.Backend.Features.User;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapUsersEndpoints();

app.MapGet("/", () => Results.Ok(new { greetingMessage = "Welcome to Aprily" }))
    .WithName("Greeting");

app.Run();
