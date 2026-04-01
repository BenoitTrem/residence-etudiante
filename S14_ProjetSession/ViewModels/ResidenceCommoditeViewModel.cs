using S14_ProjetSession.Models;

namespace S14_ProjetSession.ViewModels
{
    public class ResidenceCommoditeViewModel
    {
        public CheckBoxViewModel CheckBoxViewModel { get; set; } = new CheckBoxViewModel();
        public CommoditeDescriptionViewModel DescriptionViewModel { get; set; } = new CommoditeDescriptionViewModel();

        public ResidenceCommoditeViewModel() { }

        public ResidenceCommoditeViewModel(Commodite commodite, bool selectionne = false, string? description = null)
        {
            CheckBoxViewModel = new CheckBoxViewModel
            {
                Value = commodite.Id.ToString(),
                Label = commodite.Nom ?? "",
                IsChecked = selectionne
            };

            DescriptionViewModel = new CommoditeDescriptionViewModel
            {
                Id = commodite.Id,
                Description = description
            };
        }
    }
}
