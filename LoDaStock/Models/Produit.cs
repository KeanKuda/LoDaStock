// Déclaration du namespace pour organiser le code lié aux modèles de données de l'application LoDaStock.
namespace LoDaStock.Models
{
    // Définition de la classe 'Produit', qui représente un produit dans le système de gestion des stocks.
    public class Produit
    {
        // Identifiant unique du produit (clé primaire dans la base de données).
        public int ProduitID { get; set; }

        // Nom du produit (peut être nul, d'où la présence du '?').
        public string? NomProduit { get; set; }

        // Champ optionnel pour ajouter un commentaire ou une description supplémentaire sur le produit.
        public string? Commentaire { get; set; }

        // Prix unitaire du produit, représenté avec une précision décimale.
        public decimal PrixUnitaire { get; set; }

        // Quantité disponible en stock pour ce produit.
        public int QuantiteDisponible { get; set; }

        // Navigation property : liste des liens entre ce produit et ses fournisseurs.
        // Indique les relations de plusieurs-à-plusieurs avec la classe ProduitFournisseur.
        public ICollection<ProduitFournisseur> ProduitFournisseurs { get; set; }
    }
}