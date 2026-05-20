using System.Reflection;

using Aprily.Api.Exceptions;
using Aprily.Api.Extensions;
using Aprily.Application;
using Aprily.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplication().AddInfrastructure(builder.Configuration);

builder.Services.AddExceptionHandler<GlobalException>();
builder.Services.AddProblemDetails();

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddCors(options => 
    options.AddDefaultPolicy(policy => 
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()));

var app = builder.Build();

app.MapEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.MapGet("/", () => Results.Ok(new { greetingMessage = "Welcome to Aprily!" })).WithName("HelloWorld");

app.Run();
