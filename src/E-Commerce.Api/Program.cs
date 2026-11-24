using E_Commerce.Application;
using E_Commerce.Application.Behaviors;
using FluentValidation;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMediatR(options =>
{
    options.RegisterServicesFromAssembly(typeof(IAssemblyMarker).Assembly);
});
builder.Services.AddValidatorsFromAssembly(typeof(IAssemblyMarker).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>),typeof(ValidationBehaviors<,>));
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
