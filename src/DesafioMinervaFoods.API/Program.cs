using DesafioMinervaFoods.Infrastructure.Configs;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers()
.AddJsonOptions(options =>
 {
     // Ignora propriedades com valor nulo no JSON de saída
     options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

     // Aceita nomes de propriedades sem diferenciar maiúsculas/minúsculas no POST/PUT
     options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;

     // Garante que o JSON de saída use camelCase (padrão de mercado)
     options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
 });
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen();
builder.Services.AddCustomizedSwagger(typeof(Program));

// Dependency Injection Configuration
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

// Seed dos clientes e formas de pagamentos
await app.UseSeedAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCustomizedSwagger();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
