using Microsoft.AspNetCore.Mvc;
using Niusys.Docs.Core.DataStores;
using Niusys.Docs.Core.Projects;
using Niusys.Docs.Web.Models;
using System.Diagnostics;

namespace Niusys.Docs.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MkdocsDatabase _docRepositoryManager;

        public HomeController(ILogger<HomeController> logger, MkdocsDatabase docRepositoryManager)
        {
            _logger = logger;
            _docRepositoryManager = docRepositoryManager;
        }

        public IActionResult Index()
        {
            var repos = _docRepositoryManager.Search<DocProject>();
            return View(repos);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
