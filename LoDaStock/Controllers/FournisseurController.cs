using LoDaStock.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoDaStock.Controllers
{
    // Indique que ce contrôleur répond à 'api/fournisseur'
    [Route("api/[controller]")]
    [ApiController]
    public class FournisseurController : ControllerBase
    {
        // Contexte d'accès à la base de données
        private readonly LoDaStockDbContext _context;

        // Constructeur avec injection du contexte BD
        public FournisseurController(LoDaStockDbContext context)
        {
            _context = context;
        }

        // ----- GET : récupération de tous les fournisseurs -----
        // GET: api/fournisseur
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Fournisseur>>> GetFournisseurs()
        {
            // Retourne la liste complète des fournisseurs en base (asynchrone)
            return await _context.Fournisseurs.ToListAsync();
        }

        // ----- GET : récupération d'un fournisseur selon son id -----
        // GET: api/fournisseur/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Fournisseur>> GetFournisseur(int id)
        {
            // Recherche le fournisseur par identifiant
            var fournisseur = await _context.Fournisseurs.FindAsync(id);

            // Retourne 404 si le fournisseur n'existe pas
            if (fournisseur == null)
            {
                return NotFound();
            }

            // Retourne le fournisseur trouvé
            return fournisseur;
        }

        // ----- POST : ajout d'un nouveau fournisseur -----
        // POST: api/fournisseur
        [HttpPost]
        public async Task<ActionResult<Fournisseur>> PostFournisseur(Fournisseur fournisseur)
        {
            // Ajoute le fournisseur au contexte
            _context.Fournisseurs.Add(fournisseur);
            // Enregistre les modifications en base de données (asynchrone)
            await _context.SaveChangesAsync();

            // Retourne un 201 (Created) avec l'URL du fournisseur créé
            return CreatedAtAction(nameof(GetFournisseur), new { id = fournisseur.FournisseurID }, fournisseur);
        }

        // ----- PUT : modification d'un fournisseur existant -----
        // PUT: api/fournisseur/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFournisseur(int id, Fournisseur fournisseur)
        {
            // Vérifie cohérence id dans URL et objet
            if (id != fournisseur.FournisseurID)
            {
                return BadRequest();
            }

            // Marque l'entité comme modifiée dans le contexte EF
            _context.Entry(fournisseur).State = EntityState.Modified;

            try
            {
                // Tente d'enregistrer la modification
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Si aucune correspondance, renvoie 404
                if (!FournisseurExists(id))
                {
                    return NotFound();
                }
                else
                {
                    // Sinon relance l'exception
                    throw;
                }
            }

            // Retourne 204 (NoContent) en cas de succès
            return NoContent();
        }

        // ----- PUT : modification du statut d'un fournisseur -----
        // PUT: api/fournisseur/5/statut
        [HttpPut("{id}/statut")]
        public async Task<IActionResult> ChangerStatut(int id, [FromBody] bool nouveauStatut)
        {
            // Recherche du fournisseur dans la base
            var fournisseur = await _context.Fournisseurs.FindAsync(id);
            if (fournisseur == null)
            {
                return NotFound();
            }

            // Mise à jour du statut (valeur booléenne)
            fournisseur.statutFour = nouveauStatut;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ----- DELETE : suppression d'un fournisseur -----
        // DELETE: api/fournisseur/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFournisseur(int id)
        {
            // Recherche du fournisseur à supprimer
            var fournisseur = await _context.Fournisseurs.FindAsync(id);
            if (fournisseur == null)
            {
                return NotFound();
            }

            // Suppression dans le contexte puis persistance en base
            _context.Fournisseurs.Remove(fournisseur);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ----- Vérification existence d'un fournisseur à partir d'un id -----
        private bool FournisseurExists(int id)
        {
            // Retourne vrai si un fournisseur avec cet id existe
            return _context.Fournisseurs.Any(e => e.FournisseurID == id);
        }
    }
}