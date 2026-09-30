using Namaa.Api.Middleware;
using Namaa.Application;
 
var builder = WebApplication.CreateBuilder(args);

// The API composes the layers; use cases and persistence details stay outside HTTP endpoints.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddAuthorization();
builder.Services.AddApplication();
 
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();
app.UseAuthorization();

app.MapControllers();

app.Run();
