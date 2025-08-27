using LoDaStock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoDaStock.Controllers
{
    // Le contrôleur gère les routes sous "api/produit"
    [Route("api/[controller]")]
    [ApiController]
    public class ProduitController : ControllerBase
    {
        // Référence au contexte de base de données
        private readonly LoDaStockDbContext _context;

        // Constructeur avec injection du contexte BD
        public ProduitController(LoDaStockDbContext context)
        {
            _context = context;
        }

        // ----- GET: Récupère la liste de tous les produits, avec leurs fournisseurs -----
        // GET: api/produit
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetProduits()
        {
            // Récupère tous les produits avec les fournisseurs liés via la table de jointure
            var produits = await _context.Produits
                .Include(p => p.ProduitFournisseurs) // inclut la liaison produit-fournisseurs
                .ThenInclude(pf => pf.Fournisseur)   // inclut les fournisseurs
                .ToListAsync();

            // Construit un objet pour chaque produit avec tous les détails + ses fournisseurs
            var results = produits.Select(p => new
            {
                produitID = p.ProduitID,
                nomProduit = p.NomProduit,
                commentaire = p.Commentaire,
                prixUnitaire = p.PrixUnitaire,
                quantiteDisponible = p.QuantiteDisponible,
                fournisseurs = p.ProduitFournisseurs.Select(pf => new
                {
                    fournisseurID = pf.FournisseurID,
                    nomFournisseur = pf.Fournisseur.NomFournisseur
                }).ToList()
            });

            return Ok(results); // retourne la liste formatée
        }

        // ----- GET: Récupère un produit précis et ses fournisseurs -----
        // GET: api/produit/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetProduit(int id)
        {
            // Récupère le produit avec ses fournisseurs selon son id
            var p = await _context.Produits
                .Include(prod => prod.ProduitFournisseurs)
                .ThenInclude(pf => pf.Fournisseur)
                .FirstOrDefaultAsync(prod => prod.ProduitID == id);

            if (p == null)
            {
                return NotFound();
            }

            // Crée l'objet résultat détaillé du produit trouvé
            var result = new
            {
                produitID = p.ProduitID,
                nomProduit = p.NomProduit,
                commentaire = p.Commentaire,
                prixUnitaire = p.PrixUnitaire,
                quantiteDisponible = p.QuantiteDisponible,
                fournisseurs = p.ProduitFournisseurs.Select(pf => new
                {
                    fournisseurID = pf.FournisseurID,
                    nomFournisseur = pf.Fournisseur.NomFournisseur
                }).ToList()
            };

            return Ok(result); // retourne le produit demandé
        }

        // ----- GET: Récupère tous les fournisseurs d'un produit donné -----
        // GET: api/Produit/{produitId}/fournisseurs
        [HttpGet("{id}/fournisseurs")]
        public async Task<ActionResult> GetFournisseursForProduit(int id)
        {
            // Sélectionne la liste des fournisseurs associés à ce produit
            var fournisseurs = await _context.ProduitFournisseurs
                .Where(pf => pf.ProduitID == id)
                .Select(pf => new
                {
                    pf.Fournisseur.FournisseurID,
                    pf.Fournisseur.NomFournisseur
                })
                .Distinct() // pour s'assurer de l'unicité
                .ToListAsync();
            return Ok(fournisseurs);
        }

        // ----- Classe DTO pour sérialiser/désérialiser lors de la création/modif produit -----
        public class ProduitDto
        {
            public string NomProduit { get; set; }
            public string Commentaire { get; set; }
            public decimal PrixUnitaire { get; set; }
            public int QuantiteDisponible { get; set; }
            public List<int> FournisseurIDs { get; set; }
        }

        // ----- POST: Ajoute un nouveau produit et ses associations fournisseurs -----
        // POST: api/produit
        [HttpPost]
        public async Task<ActionResult<Produit>> PostProduit(ProduitDto dto)
        {
            // Instancie le produit à partir du DTO
            var produit = new Produit
            {
                NomProduit = dto.NomProduit,
                Commentaire = dto.Commentaire,
                PrixUnitaire = dto.PrixUnitaire,
                QuantiteDisponible = dto.QuantiteDisponible,
                ProduitFournisseurs = new List<ProduitFournisseur>()
            };

            // Crée les liens vers chaque fournisseur sélectionné
            foreach (var fournisseurID in (dto.FournisseurIDs ?? new List<int>()).Distinct())
            {
                var fournisseur = await _context.Fournisseurs.FindAsync(fournisseurID);
                if (fournisseur == null)
                    return BadRequest($"FournisseurID inexistant : {fournisseurID}");

                produit.ProduitFournisseurs.Add(new ProduitFournisseur
                {
                    FournisseurID = fournisseurID
                });
            }

            // Ajoute le produit en base
            _context.Produits.Add(produit);
            await _context.SaveChangesAsync();

            // Retourne 201 avec l'emplacement du nouveau produit
            return CreatedAtAction(nameof(GetProduit), new { id = produit.ProduitID }, produit);
        }

        // ----- PUT: Met à jour un produit existant et ses fournisseurs -----
        // PUT: api/produit/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduit(int id, ProduitDto dto)
        {
            // Récupère le produit concerné et ses associations existantes
            var produit = await _context.Produits
                .Include(p => p.ProduitFournisseurs)
                .FirstOrDefaultAsync(p => p.ProduitID == id);

            if (produit == null)
                return NotFound();

            // Met à jour les propriétés simples
            produit.NomProduit = dto.NomProduit;
            produit.Commentaire = dto.Commentaire;
            produit.PrixUnitaire = dto.PrixUnitaire;
            produit.QuantiteDisponible = dto.QuantiteDisponible;

            // Supprime toutes les liaisons "ProduitFournisseur" existantes pour ce produit
            _context.ProduitFournisseurs.RemoveRange(produit.ProduitFournisseurs);

            // Ajoute la nouvelle liste de fournisseurs (sans doublon)
            foreach (var fournisseurID in dto.FournisseurIDs.Distinct())
            {
                var fournisseur = await _context.Fournisseurs.FindAsync(fournisseurID);
                if (fournisseur == null)
                    return BadRequest($"FournisseurID inexistant : {fournisseurID}");
                produit.ProduitFournisseurs.Add(new ProduitFournisseur
                {
                    ProduitID = produit.ProduitID,
                    FournisseurID = fournisseurID
                });
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ----- DELETE: Supprime un produit (et ses associations) -----
        // DELETE: api/produit/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduit(int id)
        {
            // Récupère le produit avec ses associations
            var produit = await _context.Produits
                .Include(p => p.ProduitFournisseurs)
                .FirstOrDefaultAsync(p => p.ProduitID == id);

            if (produit == null)
            {
                return NotFound();
            }

            // Supprime d'abord les associations en base
            _context.ProduitFournisseurs.RemoveRange(produit.ProduitFournisseurs);
            // Supprime ensuite le produit lui-même
            _context.Produits.Remove(produit);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ----- Vérification d'existence d'un produit -----
        private bool ProduitExists(int id)
        {
            return _context.Produits.Any(e => e.ProduitID == id);
        }
    }
}