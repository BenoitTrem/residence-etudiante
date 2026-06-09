using Microsoft.AspNetCore.Mvc;
using ResidenceEtudiante.Data;

namespace ResidenceEtudiante.Controllers
{
    public class SemestreController : Controller
    {
        private readonly ISemestreRepository _repository;
        public SemestreController(ISemestreRepository semestreRepository)
        {
            _repository = semestreRepository;
        }
        public ViewResult Index()
        {
            return View("", _repository.Semestres);
        }

    }
}
