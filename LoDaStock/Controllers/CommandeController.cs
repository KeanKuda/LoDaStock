using LoDaStock.Data;
using LoDaStock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoDaStock.Controllers
{
    // Spécifie la route API : api/commande
    [Route("api/[controller]")]
    // Indique que ce contrôleur utilise le comportement API (validation automatique, etc.)
    [ApiController]
    public class CommandeController : ControllerBase
    {
        // Référence au contexte de base de données pour accéder aux tables
        private readonly LoDaStockDbContext _context;

        // Constructeur avec injection de dépendance du contexte
        public CommandeController(LoDaStockDbContext context)
        {
            _context = context;
        }

        // ----- GET : récupération de toutes les commandes -----
        // GET: api/commande
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommandeDto>>> GetCommandes()
        {
            // Récupère toutes les commandes, y compris leur produit et le(s) fournisseur(s) associé(s)
            var commandes = await _context.Commandes
                .Include(c => c.Produit) // Inclut le produit lié à la commande
                    .ThenInclude(p => p.ProduitFournisseurs) // Inclut la liaison produit-fournisseurs
                        .ThenInclude(pf => pf.Fournisseur) // Inclut les fournisseurs
                                                           // Projette le résultat dans un DTO pour transmettre uniquement les infos utiles
                .Select(c => new CommandeDto
                {
                    CommandeID = c.CommandeID,
                    DateCommande = c.DateCommande,
                    QuantiteCommandee = c.QuantiteCommandee,
                    ProduitID = c.ProduitID,
                    NomProduit = c.Produit.NomProduit, // Extrait le nom du produit
                    Statut = c.Statut,
                    // Prend le nom du premier fournisseur du produit (s’il y en a plusieurs)
                    FournisseurNom = c.Produit.ProduitFournisseurs.FirstOrDefault().Fournisseur.NomFournisseur
                })
                .ToListAsync(); // Exécute la requête et retourne le résultat sous forme de liste asynchrone

            return commandes;
        }

        // ----- GET : récupération d'une commande par son identifiant -----
        // GET: api/commande/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Commande>> GetCommande(int id)
        {
            // Recherche la commande correspondante à l'id
            var commande = await _context.Commandes.FindAsync(id);

            // Retourne 404 si la commande n'existe pas
            if (commande == null)
            {
                return NotFound();
            }

            // Retourne la commande trouvée
            return commande;
        }

        // ----- POST : création d'une nouvelle commande -----
        // POST: api/commande
        [HttpPost]
        public async Task<ActionResult<Commande>> PostCommande(Commande commande)
        {
            // Ajoute la commande à la base de données (état "ajouté" dans le contexte)
            _context.Commandes.Add(commande);

            // Mise à jour du stock pour le produit commandé
            var produit = await _context.Produits.FindAsync(commande.ProduitID);
            if (produit == null)
            {
                // Produit inexistant : on retourne une erreur
                return NotFound("Produit non trouvé");
            }
            // Incrémente la quantité disponible du produit selon la commande
            produit.QuantiteDisponible += commande.QuantiteCommandee;

            // Enregistre les changements BD
            await _context.SaveChangesAsync();

            // Retourne l'URL de la nouvelle ressource créée (CreatedAtAction)
            return CreatedAtAction(nameof(GetCommande), new { id = commande.CommandeID }, commande);
        }

        // ----- PUT : modification d'une commande existante -----
        // PUT: api/commande/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCommande(int id, Commande commande)
        {
            // Vérifie que l'id de l'URL correspond à l'id de la commande
            if (id != commande.CommandeID)
            {
                return BadRequest();
            }

            // Marque l'entité comme modifiée pour que EF Core la mette à jour
            _context.Entry(commande).State = EntityState.Modified;

            try
            {
                // Tente d'enregistrer les modifications BD
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si plus aucune commande avec cet id, retourne 404
                if (!CommandeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    // Sinon relance l'exception
                    throw;
                }
            }

            // Retourne 204 (no content) si succès
            return NoContent();
        }

        // ----- DELETE : suppression d'une commande -----
        // DELETE: api/commande/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommande(int id)
        {
            // Recherche la commande à supprimer par son identifiant
            var commande = await _context.Commandes.FindAsync(id);
            if (commande == null) return NotFound();

            // Supprime la commande du contexte
            _context.Commandes.Remove(commande);
            // Enregistre la suppression en base de données
            await _context.SaveChangesAsync();
            // Retourne 204
            return NoContent();
        }

        // ----- Fonction utilitaire : vérifie l'existence d'une commande -----
        private bool CommandeExists(int id)
        {
            // Vérifie si une commande avec cet id existe dans la base
            return _context.Commandes.Any(e => e.CommandeID == id);
        }
    }
}