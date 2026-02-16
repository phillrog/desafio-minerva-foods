using DesafioMinervaFoods.Infrastructure.Configs;
using DesafioMinervaFoods.Infrastructure.Hubs;
using System.Text.Json;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

// Configuração Segregadas
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddIdentitySetup(builder.Configuration);
builder.Services.AddPresentation();
// Swagger
builder.Services.AddSwaggerGen();
builder.Services.AddCustomizedSwagger(builder.Configuration, typeof(Program));

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

var app = builder.Build();

// Inicialização de Dados
await app.UseDbInitializationAsync();

// Pipeline
app.Use((context, next) => {
    context.Request.Scheme = "https";
    return next();
});

// 2. Swagger (Pode vir no topo)
app.UseCustomizedSwagger();
app.UseRouting();

app.UseCors("Cors");


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<OrderHub>("/orderHub");

app.Run();