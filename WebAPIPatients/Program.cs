using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Filters;
using WebAPIPatients.Context;
using WebAPIPatients.SwaggerExamples;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Crear variable para la cadena de conexión.
var connectionString = builder.Configuration.GetConnectionString("Connection");
// Registrar servicio para la conexión
builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(connectionString)
);

builder.Services.AddControllers()
    .AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Web API Patients",
        Version = "v1"
    });

    // Esto habilita filtros de ejemplo
    c.ExampleFilters();
});

// Registrar los ejemplos de request/response
builder.Services.AddSwaggerExamplesFromAssemblyOf<PatchPatientExample>();

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
