using MinimalApi.Dominio.Interfaces;
using Test.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;

namespace Api.Test.Helpers;

public class Setup{
    public const string PORT = "5001";
    public static TestContext testContext = default!;
    public static WebApplicationFactory<Startup> http = default!;
    public static HttpClient client = default;


    public static void ClassInit(TestContext testContext){
        Setup.testContext = testContext;

        Setup.http = new WebApplicationFactory<IStartup>()
            .WithWebHostBuilder(builder =>
            {
                builder.UseSetting("https_port", Setup.PORT)
                       .UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    services.AddScoped<iAdministradorServico, AdministradorServicoMock>();
                });
            });

        Setup.client = Setup.http.CreateClient();
    }

   
}