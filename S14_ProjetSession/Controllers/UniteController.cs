using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;


namespace S14_ProjetSession.Controllers
{
    public class UniteController : Controller
    {
        private readonly IUniteRepository _uniteRepository;
        private readonly IResidenceRepository _residenceRepository;

        public UniteController(IUniteRepository uniteRepository, IResidenceRepository residenceRepository)
        {
            _uniteRepository = uniteRepository;
            _residenceRepository = residenceRepository;
        }
        public IActionResult Unites(int id)
        {
            ViewData["Title"] = "Unités";

            List<Unite> unites = _uniteRepository.GetByResidenceId(id);
            List<Unite> unitesDisponibles = unites.Where(u => u.Capacite > 0).ToList();

            Residence residence = _residenceRepository.GetById(id);

            if (residence != null)
            {
                ViewBag.Nom = residence.Nom;
                ViewBag.Adresse = residence.Adresse;
                ViewBag.NombreUnites = unitesDisponibles.Count;
            }

            return View("Unites", unitesDisponibles);
        }
    }
}
