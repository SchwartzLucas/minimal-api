using Microsoft.EntityFrameworkCore;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.DTOs;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Entidades;

#region Builder

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddScoped<iAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<iVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<DbContexto>(options => {
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))
    );
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores
app.MapPost("/administradores/login", ( [FromBody] LoginDTO loginDTO, [FromServices] iAdministradorServico administradorServico) => {
    if(administradorServico.Login(loginDTO) != null){
        return Results.Ok("Login com sucesso");
    }else {
        return Results.Unauthorized();
    }
}).WithTags("Administradores");
#endregion

#region Veiculos
app.MapPost("/veiculos/login", ( [FromBody] VeiculoDTO veiculoDTO, [FromServices] iVeiculoServico veiculoServico) => {
    
    var veiculo = new Veiculo{
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    
    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);

}).WithTags("Veiculos");

app.MapGet("/veiculos", ( [FromQuery] int? pagina, iVeiculoServico veiculoServico) => {
    
    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);

}).WithTags("Veiculos");


app.MapGet("/veiculos/{id}", ( [FromRoute] int id, iVeiculoServico veiculoServico) => {
    
    var veiculo = veiculoServico.BuscaPorId(id);

    if(veiculo == null) return Results.NotFound();

    return Results.Ok(veiculo);

}).WithTags("Veiculos")

#endregion

#region App
    app.UseSwagger();
    app.UseSwaggerUI();

app.Run();
#endregion