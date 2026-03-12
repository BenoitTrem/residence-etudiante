using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Models;
using S14_ProjetSession.Data;


namespace S14_ProjetSession.Controllers
{
    public class EtudiantController : Controller
    {

        private readonly IEtudiantRepository _etudiantRepository;


      
        public EtudiantController(IEtudiantRepository etudiantRepository)
        {
            _etudiantRepository = etudiantRepository;
       
        }

  
        public ViewResult Index()
        {
            ViewData["Title"] = "Etudiant";
            return View("Etudiants", _etudiantRepository.Etudiants);
        }

       
        public ViewResult Creer()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Creer(Etudiant etudiant)
        {
            if (ModelState.IsValid)
            {
                _etudiantRepository.Creer(etudiant);
                return RedirectToAction("Index");
            }
            else
            {
                return View(etudiant);
            }
        }


        public ActionResult Modifier(int id)
        {
            Etudiant? etudiant = _etudiantRepository.GetEtudiant(id);
            return etudiant is null ? NotFound() : View(etudiant);
        }

    
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Modifier(Etudiant etudiant)
        {
            if (ModelState.IsValid)
            {
                _etudiantRepository.Modifier(etudiant);
                return RedirectToAction("Index");
            }
            else
            {
                return View(etudiant);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Supprimer(int etudiantId)
        {
            Etudiant etudiant = _etudiantRepository.GetEtudiant(etudiantId);
            if (etudiant is null)
            {
                return NotFound();
            }
            else
            {
                _etudiantRepository.Supprimer(etudiant);
                return RedirectToAction("Index");
            }
        }
    }
}
