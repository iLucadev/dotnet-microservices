using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Inventory.Service.Clients.Config;
using Play.Inventory.Service.Entities;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services
.AddMongo()
.AddMongoRepository<InventoryItem>("inventoryitems")
.AddMongoRepository<CatalogItem>("catalogitems")
.AddMassTransitWithRabbitMq();
CatalogClientConfig.AddCatalogClient(services);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Play.Inventory.Service", Version = "v1.0.0" });
});

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