using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Dominio.Servicos;

public class AdministradorServico : iAdministradorServico{

    private readonly DbContexto _contexto;
    public AdministradorServico(DbContexto contexto){
        _contexto = contexto;
    }
    public Administrador? Login(LoginDTO loginDTO)
    {
        var adm = _contexto.Administradores.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
        return adm;
        
    }
    public Administrador Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();

        return administrador;
        
    }
    public List<Administrador> Todos(int? pagina)
    {
        var query = _contexto.Administradores.AsQueryable();
        int ItensPorPagina = 10;

        if (pagina.HasValue && pagina.Value > 0)
        {
            int p = (pagina.Value - 1) * ItensPorPagina;
            query = query.Skip(p).Take(ItensPorPagina);
        }
        else
        {
            query = query.Take(ItensPorPagina);
        }

        return query.ToList();
    }

    public Administrador? BuscaPorId(int id)
    {
        return _contexto.Administradores.Where(v => v.Id == id).FirstOrDefault();
    }


}