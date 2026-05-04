using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using S14_ProjetSession.Models;

namespace S14_ProjetSession
{
    /// <summary>
    /// @author Felix
    /// rendu BEAU A L'AIDE DE COPILOTE PDF Logique par FELIX
    /// Genere les documents PDF des demandes de residence.
    /// </summary>
    public static class DemandePdf
    {
        public static byte[] Generate(Demande demande)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter);
                    page.Margin(35);
                    page.DefaultTextStyle(text => text.FontSize(10).FontColor(Colors.Grey.Darken4));

                    page.Header().Element(header => ComposerEntete(header, "Demande de residence", $"No {demande.Id}"));

                    page.Content().PaddingVertical(15).Column(col =>
                    {
                        col.Spacing(14);
                        col.Item().Element(c => ComposerBlocResume(c, demande));
                        col.Item().Element(c => ComposerSectionEtudiant(c, demande));
                        col.Item().Element(c => ComposerSectionDemande(c, demande));
                        col.Item().Element(c => ComposerSectionContacts(c, demande));
                        col.Item().Element(c => ComposerSectionJumelages(c, demande));
                    });

                    page.Footer().Element(ComposerPiedPage);
                });
            }).GeneratePdf();
        }

        public static byte[] GenerateToutes(IEnumerable<Demande> demandes)
        {
            List<Demande> listeDemandes = demandes
                .OrderByDescending(d => d.DateDemande)
                .ToList();

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.Letter.Landscape());
                    page.Margin(28);
                    page.DefaultTextStyle(text => text.FontSize(9).FontColor(Colors.Grey.Darken4));

                    page.Header().Element(header => ComposerEntete(header, "Export des demandes", $"{listeDemandes.Count} demande(s)"));

                    page.Content().PaddingVertical(12).Column(col =>
                    {
                        col.Spacing(10);
                        col.Item().Text($"Document genere le {DateTime.Now:yyyy-MM-dd HH:mm}")
                            .FontColor(Colors.Grey.Darken2);

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(2.2f);
                                columns.RelativeColumn(1.8f);
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(1.5f);
                                columns.RelativeColumn(2.2f);
                                columns.RelativeColumn(1.8f);
                            });

                            AjouterCelluleEntete(table, "No");
                            AjouterCelluleEntete(table, "Etudiant");
                            AjouterCelluleEntete(table, "Semestre");
                            AjouterCelluleEntete(table, "Statut");
                            AjouterCelluleEntete(table, "Date");
                            AjouterCelluleEntete(table, "Genres");
                            AjouterCelluleEntete(table, "Unite");

                            foreach (Demande demande in listeDemandes)
                            {
                                AjouterCellule(table, demande.Id.ToString());
                                AjouterCellule(table, NomEtudiant(demande));
                                AjouterCellule(table, demande.Semestre?.NomSemestre ?? "Non precise");
                                AjouterCellule(table, StatutTexte(demande));
                                AjouterCellule(table, demande.DateDemande.ToString("yyyy-MM-dd"));
                                AjouterCellule(table, GenresTexte(demande));
                                AjouterCellule(table, UniteTexte(demande));
                            }
                        });
                    });

                    page.Footer().Element(ComposerPiedPage);
                });
            }).GeneratePdf();
        }

        private static void ComposerEntete(IContainer container, string titre, string sousTitre)
        {
            container.Background(Colors.Blue.Darken3).Padding(16).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text(titre).FontSize(22).Bold().FontColor(Colors.White);
                    col.Item().Text("Residences etudiantes").FontSize(10).FontColor(Colors.Blue.Lighten4);
                });

                row.ConstantItem(140).AlignRight().AlignMiddle().Text(sousTitre)
                    .FontSize(12).SemiBold().FontColor(Colors.White);
            });
        }

        private static void ComposerBlocResume(IContainer container, Demande demande)
        {
            container.Background(Colors.Grey.Lighten4).Border(1).BorderColor(Colors.Grey.Lighten2).Padding(12).Row(row =>
            {
                AjouterResume(row, "Statut", StatutTexte(demande));
                AjouterResume(row, "Semestre", demande.Semestre?.NomSemestre ?? "Non precise");
                AjouterResume(row, "Date", demande.DateDemande.ToString("yyyy-MM-dd"));
                AjouterResume(row, "Bail", $"{demande.PrefDureeBail} jours");
            });
        }

        private static void ComposerSectionEtudiant(IContainer container, Demande demande)
        {
            container.Column(col =>
            {
                AjouterTitreSection(col, "Etudiant");
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        AjouterLigne(c, "Nom", NomEtudiant(demande));
                        AjouterLigne(c, "No etudiant", demande.Etudiant?.noEtudiant ?? "Non precise");
                        AjouterLigne(c, "Telephone", demande.Etudiant?.Telephone ?? "Non precise");
                    });

                    row.RelativeItem().Column(c =>
                    {
                        AjouterLigne(c, "Courriel institutionnel", demande.Etudiant?.CourrielInstitutionnel ?? "Non precise");
                        AjouterLigne(c, "Courriel personnel", demande.Etudiant?.CourrielPersonnel ?? "Non precise");
                        AjouterLigne(c, "Mobilite reduite", demande.Etudiant?.MobiliteReduite == true ? "Oui" : "Non");
                    });
                });
            });
        }

        private static void ComposerSectionDemande(IContainer container, Demande demande)
        {
            container.Column(col =>
            {
                AjouterTitreSection(col, "Details de la demande");
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        AjouterLigne(c, "Genres preferes", GenresTexte(demande));
                        AjouterLigne(c, "Reglements acceptes", demande.AccepteReglements ? "Oui" : "Non");
                        AjouterLigne(c, "Traitement des donnees", demande.AccepteTraitementDonnees ? "Oui" : "Non");
                    });

                    row.RelativeItem().Column(c =>
                    {
                        AjouterLigne(c, "Soumission confirmee", demande.ConfirmeSoumission ? "Oui" : "Non");
                        AjouterLigne(c, "Date debut bail", demande.DateDebutBail?.ToString("yyyy-MM-dd") ?? "Non precise");
                        AjouterLigne(c, "Date fin bail", demande.DateFinBail?.ToString("yyyy-MM-dd") ?? "Non precise");
                        AjouterLigne(c, "Unite assignee", UniteTexte(demande));
                    });
                });
            });
        }

        private static void ComposerSectionContacts(IContainer container, Demande demande)
        {
            container.Column(col =>
            {
                AjouterTitreSection(col, "Garant, parent et urgence");
                col.Item().Row(row =>
                {
                    row.RelativeItem().Column(c =>
                    {
                        AjouterLigne(c, "Garant", $"{demande.PrenomGarant} {demande.NomGarant}".Trim());
                        AjouterLigne(c, "Naissance garant", demande.DateNaissanceGarant?.ToString("yyyy-MM-dd") ?? "Non precise");
                        AjouterLigne(c, "Courriel garant", demande.CourrielGarant);
                        AjouterLigne(c, "Telephone garant", demande.TelephoneGarant);
                    });

                    row.RelativeItem().Column(c =>
                    {
                        AjouterLigne(c, "Parent", demande.NomParent ?? "Non precise");
                        AjouterLigne(c, "Courriel parent", demande.CourrielParent ?? "Non precise");
                        AjouterLigne(c, "Contact urgence", demande.NomUrgence);
                        AjouterLigne(c, "Lien urgence", demande.LienParenteUrgence);
                        AjouterLigne(c, "Telephone urgence", demande.TelephoneUrgence);
                    });
                });
            });
        }

        private static void ComposerSectionJumelages(IContainer container, Demande demande)
        {
            container.Column(col =>
            {
                AjouterTitreSection(col, "Jumelages");

                if (demande.Jumelages == null || !demande.Jumelages.Any())
                {
                    col.Item().Text("Aucun jumelage indique.").Italic().FontColor(Colors.Grey.Darken1);
                    return;
                }

                foreach (Jumelage jumelage in demande.Jumelages)
                {
                    col.Item().Text($"{jumelage.Nom} - {jumelage.Courriel}");
                }
            });
        }

        private static void ComposerPiedPage(IContainer container)
        {
            container.AlignCenter().Text(text =>
            {
                text.Span("Document genere automatiquement - ");
                text.CurrentPageNumber();
                text.Span(" / ");
                text.TotalPages();
            });
        }

        private static void AjouterTitreSection(ColumnDescriptor col, string titre)
        {
            col.Item().PaddingBottom(5).Text(titre).FontSize(14).SemiBold().FontColor(Colors.Blue.Darken3);
            col.Item().LineHorizontal(1).LineColor(Colors.Blue.Lighten3);
        }

        private static void AjouterLigne(ColumnDescriptor col, string libelle, string valeur)
        {
            col.Item().PaddingVertical(2).Text(text =>
            {
                text.Span($"{libelle} : ").SemiBold();
                text.Span(string.IsNullOrWhiteSpace(valeur) ? "Non precise" : valeur);
            });
        }

        private static void AjouterResume(RowDescriptor row, string libelle, string valeur)
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text(libelle).FontSize(8).FontColor(Colors.Grey.Darken1);
                col.Item().Text(valeur).FontSize(11).SemiBold();
            });
        }

        private static void AjouterCelluleEntete(TableDescriptor table, string texte)
        {
            table.Cell().Background(Colors.Blue.Darken3).Padding(6).Text(texte).FontColor(Colors.White).SemiBold();
        }

        private static void AjouterCellule(TableDescriptor table, string texte)
        {
            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Text(texte);
        }

        private static string NomEtudiant(Demande demande)
        {
            return $"{demande.Etudiant?.Prenom} {demande.Etudiant?.Nom}".Trim();
        }

        private static string GenresTexte(Demande demande)
        {
            if (demande.DemandeGenres == null || !demande.DemandeGenres.Any())
            {
                return "Non precise";
            }

            return string.Join(", ", demande.DemandeGenres
                .Where(dg => dg.Genre != null)
                .Select(dg => dg.Genre!.Nom));
        }

        private static string StatutTexte(Demande demande)
        {
            return demande.StatutDemande switch
            {
                StatutDemande.Acceptee => "Acceptee",
                StatutDemande.Refusee => "Refusee",
                _ => "En attente"
            };
        }

        private static string UniteTexte(Demande demande)
        {
            if (demande.Unite == null)
            {
                return "Non assignee";
            }

            return $"{demande.Unite.Residence?.Nom ?? "Residence"} - unite {demande.Unite.Numero}";
        }
    }
}
