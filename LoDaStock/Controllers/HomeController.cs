using System.Diagnostics;
using LoDaStock.Models;
using Microsoft.AspNetCore.Mvc;

namespace LoDaStock.Controllers
{
    // Contrôleur principal pour les vues de base comme l'accueil ou la confidentialité
    public class HomeController : Controller
    {
        // Logger pour journaliser les informations, avertissements ou erreurs
        private readonly ILogger<HomeController> _logger;

        // Constructeur avec injection du logger
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Action pour la page d'accueil. Renvoie la vue Index.
        public IActionResult Index()
        {
            return View();
        }

        // Action pour la page de Politique de confidentialité. Renvoie la vue Privacy.
        public IActionResult Privacy()
        {
            return View();
        }

        // Action pour la gestion des erreurs. La réponse ne doit pas être mise en cache
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Crée et retourne la vue Error avec le modèle ErrorViewModel,
            // utilisant l'identifiant de requête courant (pour le suivi/debug)
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}