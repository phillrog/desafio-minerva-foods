using DesafioMinervaFoods.Infrastructure.Configs;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen();
builder.Services.AddCustomizedSwagger(typeof(Program));

// Dependency Injection Configuration
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplication();

var app = builder.Build();

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
