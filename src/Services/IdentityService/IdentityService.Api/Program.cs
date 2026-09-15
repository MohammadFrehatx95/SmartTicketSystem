using IdentityService.Infrastructure.Extensions;
using Shared.Infrastructure.Extensions;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.ConnectDatabase(builder.Configuration);
builder.Services.AddIdentityServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddJwtServices(builder.Configuration);

var app = builder.Build();

await app.SeedIdentityRolesAsync();

app.UseSharedMiddlewares(); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();