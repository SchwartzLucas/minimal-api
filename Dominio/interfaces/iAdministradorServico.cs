using MinimalApi.Dominio.Entidades;
using MinimalApi.DTOs;

namespace MinimalApi.Dominio.Interfaces;

public interface iAdministradorServico{
    Administrador? Login(LoginDTO loginDTO);
    Administrador? Incluir(AdministradorDTO administradorDTO);
    List<Administrador> Todos(int? pagina);
}

