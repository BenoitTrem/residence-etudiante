using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Text;
namespace S14_ProjetSessionTests.Integration
{
    public class RoutageTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        private readonly HttpClient _client;

        public RoutageTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddScoped<IDemandeRepository, MockDemandeRepository>();
                    services.AddScoped<IEtudiantRepository, MockEtudiantRepository>();
                });
                builder.UseEnvironment("Test");
            });
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }
        // Un test de route pour vérifier qu’une route est disponible
        [Fact]
        public async Task DemandeControllerEtat() 
        {
            HttpResponseMessage responseMessage = await _client.GetAsync("/demande", TestContext.Current.CancellationToken);
            Assert.True(responseMessage.IsSuccessStatusCode);
        }

       
        // s'assurer que les demande crée sont bien afficher Dans /demande/demandes
        
        
        // Test - Un test de vue pour vérifier qu’une propriété de l’objet s’affiche dans la vue


        //- Un test pour vérifier que la création d’un objet s’effectue avec des données valides

        

        // - Un test pour vérifier que la création d’un objet est refusée avec des données invalides


        // - Un test pour vérifier que la modification d’un objet s’effectue avec des données valides

        // - Un test pour vérifier que la modification d’un objet est refusée avec des données invalides

        // - Un test pour vérifier la suppression d’un objet

        // - Un test pour vérifier la modification de la relation d’un objet

        //- Deux tests pour vérifier qu’une route n’est pas accessible aux utilisateurs qui ne sont pas connectés
        //(un test qui vérifie que la route est accessible à l’utilisateur connecté, un test qui vérifie que la même
        //route n’est pas accessible à l’utilisateur qui n’est pas connecté)

        //Deux tests pour vérifier une règle d’autorisation qui utilise un rôle (ex. un test pour vérifier que seul un
        //admin peut supprimer une voiture). Un test vérifie que l’utilisateur qui n’a pas le rôle nécessaire ne
        //peut effectuer l’opération.L’autre test vérifie que la même fonctionnalité est accessible aux
        //utilisateurs possédant les permissions appropriées.

    }
}
