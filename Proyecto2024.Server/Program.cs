using Microsoft.EntityFrameworkCore;
using Proyecto2024.BD.Data;
using Proyecto2024.Server.Repositorio;
using System.Text.Json.Serialization;

//------------------------------------------------------------------
// Configuracion de los servicios en el constructor de la aplicación
var builder = WebApplication.CreateBuilder(args);


// Nugget de Redis - Servicio de Cache en Memoria 
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.InstanceName = "Proyecto2024_";
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});


// 1 - Cración del Constructor: Agregamos Cache al Proyecto, Tiempo 40 Segundos.
builder.Services.AddOutputCache(options =>

{ options.DefaultExpirationTimeSpan = TimeSpan.FromSeconds(40);
}
);

builder.Services.AddControllers().AddJsonOptions(
    x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<Context>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<Context>(op => op.UseSqlServer("name=conn"));

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddScoped<ITituloRepositorio, TituloRepositorio>();
builder.Services.AddScoped<ITDocumentoRepositorio, TDocumentoRepositorio>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
//--------------------------------------------------------------------
//construccón de la aplicación
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();

app.UseAuthorization();
// 2 - Se agrega el UseOutputCache para el constructor/servicio 
app.UseOutputCache();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
