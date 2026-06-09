using ResidenceEtudiante.Models;

namespace ResidenceEtudiante.ViewModels;

/// <author>Felix</author>
public class DemandeCreateViewModel
{
    public Demande Demande { get; set; } = new();

    public List<JumelageViewModel> Jumelages { get; set; } = new()
    {
        new JumelageViewModel(),
        new JumelageViewModel(),
        new JumelageViewModel()
    };

    // Dropdowns
    public IEnumerable<Genre> Genres { get; set; } = new List<Genre>();
    public IEnumerable<Semestre> Semestres { get; set; } = new List<Semestre>();

    // Sélections choisies
    public List<int> SelectedGenreIds { get; set; } = new();
    public int? SelectedSemestreId { get; set; }
}
