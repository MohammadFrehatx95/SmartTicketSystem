using Shared.Infrastructure.Extensions;
using TicketService.Api.Extensions;
using TicketService.Application.Extensions;
using TicketService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();

builder.Services.ConnectDataBase(builder.Configuration);

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddApplicationServices();

builder.Services.AddJwtAuthentication(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSharedMiddlewares();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
