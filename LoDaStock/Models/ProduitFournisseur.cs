// Déclaration du namespace pour organiser le code lié aux modèles de données de l'application LoDaStock.
namespace LoDaStock.Models
{
    // Définition de la classe 'ProduitFournisseur'.
    // Cette classe représente la table de liaison pour une relation plusieurs-à-plusieurs entre les produits et les fournisseurs.
    public class ProduitFournisseur
    {
        // Identifiant du produit associé (clé étrangère vers Produit).
        public int ProduitID { get; set; }
        // Propriété de navigation permettant d'accéder à l'objet Produit lié.
        public Produit Produit { get; set; }

        // Identifiant du fournisseur associé (clé étrangère vers Fournisseur).
        public int FournisseurID { get; set; }
        // Propriété de navigation permettant d'accéder à l'objet Fournisseur lié.
        public Fournisseur Fournisseur { get; set; }
    }
}