using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;
using System.Security.Claims;

namespace S14_ProjetSessionTests.Integration.DemandeTests
{
    public class ModifierDemandeTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private IDemandeRepository _demandeRepository = new MockDemandeRepository();
        private ISemestreRepository _semestreRepository = new MockSemestreRepository();
        private IEtudiantRepository _etudiantRepository = new MockEtudiantRepository();
        private IGenresRepository _genresRepository = new MockGenreRepository();
        private ClaimsPrincipal? _currentUser;
        private ClaimsPrincipal _utilisateur = AuthUtilities.CreerEtudiant();

        public ModifierDemandeTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IDemandeRepository>(_demandeRepository);
                    services.AddSingleton<IEtudiantRepository>(_etudiantRepository);
                    services.AddSingleton<ISemestreRepository>(_semestreRepository);
                    services.AddSingleton<IGenresRepository>(_genresRepository);

                    services.AddSingleton<Func<ClaimsPrincipal?>>(() => _currentUser);
                    services.AddAuthentication("TestAuth")
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("TestAuth", o => { });
                });
                builder.UseEnvironment("Test");
            });
            _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            _currentUser = _utilisateur;
        }

        private async Task<string> ObtenirToken(string chemin)
        {
            HttpResponseMessage response = await _client.GetAsync(chemin, TestContext.Current.CancellationToken);
            string responseBody = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
            return Utils.GetToken(responseBody);
        }

        private async Task<HttpContent> GetForm(Dictionary<string, string> formData, string chemin)
        {
            string token = await ObtenirToken(chemin);
            formData["__RequestVerificationToken"] = token;
            return new FormUrlEncodedContent(formData);
        }

        [Fact(DisplayName = "vérifier que la modification d'un objet s'effectue avec des données valides")]
        public async Task VerifierModificationValide()
        {
            int semestreAvant = _demandeRepository.GetDemande(1).SemestreId;

            var formData = new Dictionary<string, string>
            {
                { "Demande.Id",                     "1"              },
                { "SelectedSemestreId",             "5"              },
                { "Demande.EtudiantId",             "1"              },
                { "SelectedGenreIds[0]",            "1"              },
                { "SelectedGenreIds[1]",            "2"              },
                { "Demande.PrefDureeBail",          "180"            },

                { "Demande.AccepteReglements",          "true"       },
                { "Demande.AccepteTraitementDonnees",   "true"       },
                { "Demande.ConfirmeSoumission",         "true"       },

                { "Demande.NomGarant",              "Tremblay"       },
                { "Demande.PrenomGarant",           "Jean"           },
                { "Demande.DateNaissanceGarant",    "1990-01-01"     },
                { "Demande.CourrielGarant",         "test@test.com"  },
                { "Demande.TelephoneGarant",        "+18191234567"   },

                { "Demande.NomParent",              "Parent Test"    },
                { "Demande.CourrielParent",         "parent@test.com"},

                { "Demande.NomUrgence",             "Urgence Test"   },
                { "Demande.LienParenteUrgence",     "Pere"           },
                { "Demande.TelephoneUrgence",       "+18199999999"   },

                { "Jumelages[0].Nom",               "Alex"           },
                { "Jumelages[0].Courriel",          "alex@test.com"  }
            };

            HttpContent form = await GetForm(formData, "/Demande/Modifier/1");

            HttpResponseMessage response = await _client.PostAsync("/Demande/Modifier", form, TestContext.Current.CancellationToken);

            Demande demandeApres = _demandeRepository.GetDemande(1)!;

            Assert.Equal(System.Net.HttpStatusCode.Redirect, response.StatusCode);
            Assert.NotEqual(semestreAvant, demandeApres.SemestreId);
            Assert.Equal(5, demandeApres.SemestreId);
            Assert.Equal(180, demandeApres.PrefDureeBail);
            Assert.Contains(demandeApres.DemandeGenres, g => g.Id == 1);
            Assert.Contains(demandeApres.DemandeGenres, g => g.Id == 2);
            Assert.Contains(demandeApres.Jumelages, j => j.Nom == "Alex" && j.Courriel == "alex@test.com");
        }

        [Fact(DisplayName = "vérifier que la modification d'un objet est refusée avec des données invalides")]
        public async Task VerifierModificationInvalideValide()
        {
            int semestreAvant = _demandeRepository.GetDemande(1)!.SemestreId;
            var genresAvant = _demandeRepository.GetDemande(1)!.DemandeGenres.Select(g => g.Id).ToList();


            var formData = new Dictionary<string, string>
            {
                { "Demande.Id",                     "1"              },
                { "SelectedSemestreId",             "68"             }, // semestre invalide → refus attendu
                { "Demande.EtudiantId",             "1"              },
                { "SelectedGenreIds[0]",            "1"              },
                { "Demande.PrefDureeBail",          "120"            },

                { "Demande.AccepteReglements",          "true"       },
                { "Demande.AccepteTraitementDonnees",   "true"       },
                { "Demande.ConfirmeSoumission",         "true"       },

                { "Demande.NomGarant",              "Tremblay"       },
                { "Demande.PrenomGarant",           "Jean"           },
                { "Demande.DateNaissanceGarant",    "1990-01-01"     },
                { "Demande.CourrielGarant",         "test@test.com"  },
                { "Demande.TelephoneGarant",        "8191234567"     },

                { "Demande.NomParent",              "Parent Test"    },
                { "Demande.CourrielParent",         "parent@test.com"},

                { "Demande.NomUrgence",             "Urgence Test"   },
                { "Demande.LienParenteUrgence",     "Pere"           },
                { "Demande.TelephoneUrgence",       "8199999999"     },

                { "Jumelages[0].Nom",               "Alex"           },
                { "Jumelages[0].Courriel",          "alex@test.com"  }
            };

            string chemin = $"/Demande/Modifier/1";
            HttpContent form = await GetForm(formData, chemin);

            HttpResponseMessage response = await _client.PostAsync(chemin, form, TestContext.Current.CancellationToken);

            Demande demandeApres = _demandeRepository.GetDemande(1)!;

            // Le semestre ne doit pas avoir changé
            Assert.Equal(semestreAvant, demandeApres.SemestreId);

            // Les genres ne doivent pas avoir changé non plus
            Assert.Equal(genresAvant, demandeApres.DemandeGenres.Select(g => g.Id).ToList());
            
        }
    }
}
