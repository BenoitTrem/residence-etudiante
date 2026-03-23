using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;

namespace S14_ProjetSession.Controllers
{
    public class ResidenceController : Controller
    {

        private readonly IResidenceRepository _residenceRepository;

        public ResidenceController(IResidenceRepository residenceRepository)
        {
            _residenceRepository = residenceRepository;
        }

        public IActionResult Index()
        {
            ViewData["Title"] = "Résidences";
            return View("Residences", _residenceRepository.GetAll());
        }
    }
}
