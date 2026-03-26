using Microsoft.AspNetCore.Mvc;
using S14_ProjetSession.Data;

namespace S14_ProjetSession.Controllers
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
