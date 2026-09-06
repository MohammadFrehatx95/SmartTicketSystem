using TicketService.Api.Extensions;
using TicketService.Application.Extensions;
using TicketService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();

builder.Services.ConnectDataBase(builder.Configuration);

builder.Services.AddInfrastructureServices();

builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
