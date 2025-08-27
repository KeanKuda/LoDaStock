using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoDaStock.Models;
using System.Threading.Tasks;
using LoDaStock.Data;

namespace LoDaStock.Controllers
{
    // Ce contrôleur gère les routes associées à "api/SortieStock"
    [Route("api/[controller]")]
    [ApiController]
    public class SortieStockController : ControllerBase
    {
        // Référence au contexte de base de données
        private readonly LoDaStockDbContext _context;

        // Constructeur avec injection du contexte BD
        public SortieStockController(LoDaStockDbContext context)
        {
            _context = context;
        }

        // ----- POST : Effectue une sortie de stock d'un produit -----
        // POST: api/SortieStock
        [HttpPost]
        public async Task<IActionResult> SortirDuStock([FromBody] SortieStockDto dto)
        {
            // Recherche du produit par son ID
            var produit = await _context.Produits.FindAsync(dto.ProduitID);
            if (produit == null)
                return NotFound("Produit introuvable"); // 404 si le produit n'existe pas

            // Vérifie si le stock du produit est suffisant pour cette sortie
            if (produit.QuantiteDisponible < dto.QuantiteSortie)
                return BadRequest("Stock insuffisant"); // 400 si quantité trop faible

            // Met à jour le stock disponible en retranchant la quantité sortie
            produit.QuantiteDisponible -= dto.QuantiteSortie;

            // Crée un enregistrement de la sortie stock
            var sortie = new SortieStock
            {
                ProduitID = dto.ProduitID,
                QuantiteSortie = dto.QuantiteSortie,
                DateSortie = DateTime.Now,
                Commentaire = dto.Commentaire
            };

            // Ajoute l'objet SortieStock à la base
            _context.SortiesStock.Add(sortie);

            // Sauvegarde les modifications dans la BD
            await _context.SaveChangesAsync();

            return Ok(); // 200 OK si la sortie a été faite
        }
    }
}