using Game.Hubs;
using Microsoft.AspNetCore.Cors;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();
builder.Services.AddControllers();

// Swagger e outros serviços
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", policy => // Corrigido para "AllowOrigin"
    {
        policy.WithOrigins("http://localhost:4200")  // URL do seu frontend
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Habilitar CORS
app.UseCors("AllowOrigin");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configurar o SignalR
app.MapHub<GameHub>("/gamehub");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
