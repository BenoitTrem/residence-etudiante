using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;
using S14_ProjetSession.Models;

[Authorize(Policy = "AdminOuGestionnaire")]
public class CommoditeController : Controller
{
    private readonly ICommoditeRepository _commoditeRepository;

    public CommoditeController(ICommoditeRepository commoditeRepository)
    {
        _commoditeRepository = commoditeRepository;
    }

    public IActionResult Index()
    {
        ViewData["Title"] = "Commodités";
        return View("Commodites", _commoditeRepository.GetAll());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Creer([Bind("Nom")] Commodite commodite)
    {
        if (ModelState.IsValid)
        {
            if (_commoditeRepository.NomExiste(commodite.Nom))
            {
                TempData["Erreur"] = "Ce nom de commodité existe déjà.";
            }
            else
            {
                _commoditeRepository.Ajouter(commodite);
                TempData["Succes"] = $"La commodité {commodite.Nom} a été ajoutée avec succès.";
            }
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Modifier([Bind("Id, Nom")] Commodite commodite)
    {
        if (ModelState.IsValid)
        {
            if (_commoditeRepository.NomExiste(commodite.Nom, commodite.Id))
            {
                TempData["Erreur"] = "Ce nom de commodité existe déjà.";
            }
            else
            {
                _commoditeRepository.Modifier(commodite);
                TempData["Succes"] = $"La commodité {commodite.Nom} a été modifiée avec succès.";
            }
        }
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Supprimer(int Id)
    {
        var commodite = _commoditeRepository.GetCommodite(Id);
        if (commodite == null)
        {
            TempData["Erreur"] = "La commodité n'existe pas.";
        }
        else
        {
            _commoditeRepository.Supprimer(commodite);
            TempData["Succes"] = $"La commodité {commodite.Nom} a été supprimée avec succès.";
        }
        return RedirectToAction("Index");
    }
}