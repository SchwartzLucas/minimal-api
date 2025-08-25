using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Entidades;

[TestClass]

public class AdministradorServicoTest{
        private DbContexto CriarContextoDeTeste(){

            var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));

            var builder = new ConfigurationBuilder()
            .SetBasePath(path ?? Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

            var configuraration = builder.Build();
        
            return new DbContexto(configuraration);
        }
        
    [TestMethod]
    public void TestandoSalvarAdministrador(){


        //Arrange - criar item

        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");
        context.ChangeTracker.Clear();

        var adm = new Administrador();
        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        var administradorServico = new AdministradorServico(context);
    
        // Act - setar propriedade, testando o set
        administradorServico.Incluir(adm);


        // Assert - validação, testando o get
        Assert.AreEqual(1, administradorServico.Todos(1).Count);
    
    }

    [TestMethod]
    public void AdmBuscaporID(){

        //Arrange - criar item
        var context = CriarContextoDeTeste();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");
        context.ChangeTracker.Clear();

        var adm = new Administrador();
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        var administradorServico = new AdministradorServico(context);
    
        // Act - setar propriedade, testando o set
        administradorServico.Incluir(adm);
        var admDoBanco = administradorServico.BuscaPorId(adm.Id);


        // Assert - validação, testando o get
        Assert.AreEqual(1, admDoBanco.Id);
    
    }
}