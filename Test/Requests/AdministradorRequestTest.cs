using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;
using System.Text.Json;
using Api.Test.Helpers;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.DTOs;

namespace Test.Request;

[TestClass]

public class AdministradorRequestTest{
    private object response;

    [ClassInitialize]

    public void ClasInit(TestContext testContext){
        Setup.ClassInit(testContext);
    }
    [ClassCleanup]
    public static void ClasCleanup(){
        Setup.ClassCleanup();
    }

    [TestMethod]
    public async Task TestarGetSetPropriedades(){
        var loginDTO = new LoginDTO{
            Email = "adm@teste.com",
            Senha = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");


        var responde = await Setup.client.PostAsync("/Administradores/login", content);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var admLogado = JsonSerializer.Deserialize<AdministradorLodago>(result, new JsonSerializerOptions{
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(admLogado.Email);
        Assert.IsNotNull(admLogado.Perfil);
        Assert.IsNotNull(admLogado.Token);
    }
}