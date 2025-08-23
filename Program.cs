using Microsoft.EntityFrameworkCore;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.DTOs;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;

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

app.MapGet("/administradores", ( [FromQuery] int? pagina, [FromServices] iAdministradorServico administradorServico) => {
    var adms = new List<AdministradorModelView>();
    var administradores = administradorServico.Todos(pagina);
    foreach(var adm in administradores){

        adms.Add(new AdministradorModelView{
            Email = adm.Email,
            Perfil = adm.Perfil,
            Id = adm.Id
        });

    }
    return Results.Ok(adms);
}).WithTags("Administradores");

app.MapGet("/administradores/{id}", ( [FromRoute] int id, iAdministradorServico administradorServico) => {
    
    var administrador = administradorServico.BuscaPorId(id);

    if(administrador == null) return Results.NotFound();

    return Results.Ok(new AdministradorModelView{
            Email = administrador.Email,
            Perfil = administrador.Perfil,
            Id = administrador.Id
        });

}).WithTags("Administradores");

app.MapPost("/administradores", ( [FromBody] AdministradorDTO administradorDTO, [FromServices] iAdministradorServico administradorServico) => {
    var validacao = new ErrosDeValidacao{
        Mensagens = new List<string>()
    };

    if(string.IsNullOrEmpty(administradorDTO.Email)){
        validacao.Mensagens.Add("Email não pode ser vazio");
    }
    if(string.IsNullOrEmpty(administradorDTO.Senha)){
        validacao.Mensagens.Add("Senha não pode ser vazia");
    }
    if(administradorDTO.Perfil == null){
        validacao.Mensagens.Add("Perfil não pode ser vazio");
    }

    if(validacao.Mensagens.Count > 0){
        return Results.BadRequest(validacao);
    }
    
    var administrador = new Administrador{
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };
    
    administradorServico.Incluir(administrador);

    return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelView{
            Email = administrador.Email,
            Perfil = administrador.Perfil,
            Id = administrador.Id
        });          
   
}).WithTags("Administradores");
#endregion

#region Veiculos

ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO) {
    var validacao = new ErrosDeValidacao();

    if(string.IsNullOrEmpty(veiculoDTO.Nome)){
        validacao.Mensagens.Add("O nome nao pode ser vazio!");
    }
    if(string.IsNullOrEmpty(veiculoDTO.Marca)){
        validacao.Mensagens.Add("A marca nao pode ficar em branco!");
    }
    if(veiculoDTO.Ano < 1950){
        validacao.Mensagens.Add("Veiculo muito antigo, aceito somente anos superiores a 1950!");
    }

    return validacao;
    
}

app.MapPost("/veiculos", ( [FromBody] VeiculoDTO veiculoDTO, [FromServices] iVeiculoServico veiculoServico) => {

    var validacao = validaDTO(veiculoDTO);
    if(validacao.Mensagens.Count > 0){
        return Results.BadRequest(validacao);
    }

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

}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ( [FromQuery] int id, VeiculoDTO veiculoDTO, iVeiculoServico veiculoServico) => {
    
    var validacao = validaDTO(veiculoDTO);
    if(validacao.Mensagens.Count > 0){
        return Results.BadRequest(validacao);
    }
    
    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);
    
    return Results.Ok(veiculo);

}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ( [FromQuery] int id, iVeiculoServico veiculoServico) => {
    
    var veiculo = veiculoServico.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();

    veiculoServico.Apagar(veiculo);
    
    return Results.NoContent();

}).WithTags("Veiculos");

#endregion

#region App
    app.UseSwagger();
    app.UseSwaggerUI();

app.Run();
#endregion