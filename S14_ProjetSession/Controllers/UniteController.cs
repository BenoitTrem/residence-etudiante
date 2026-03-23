using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;


namespace S14_ProjetSession.Controllers
{
    public class UniteController : Controller
    {
        private readonly IUniteRepository _uniteRepository;

        public UniteController(IUniteRepository uniteRepository)
        {
            _uniteRepository = uniteRepository;
        }
        public IActionResult Unites(int id)
        {
            ViewData["Title"] = "Unités";

            List<Unite> unites = _uniteRepository.GetByResidenceId(id);

            if (unites.Any())
            {
                Residence residence = unites.First().Residence;

                ViewBag.Nom = residence.Nom;
                ViewBag.Adresse = residence.Adresse;
                ViewBag.NombreUnites = unites.Count;
            }

            return View("Unites", unites);
        }
    }
}
