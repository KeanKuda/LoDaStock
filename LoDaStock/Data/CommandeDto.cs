using LoDaStock.Models;

namespace LoDaStock.Data
{
    // Définition d'une classe DTO (Data Transfer Object) pour transporter les données d'une commande.
    public class CommandeDto
    {
        // Identifiant unique de la commande
        public int CommandeID { get; set; }

        // Date à laquelle la commande a été passée
        public DateTime DateCommande { get; set; }

        // Quantité commandée pour ce produit
        public int QuantiteCommandee { get; set; }

        // Identifiant du produit associé à cette commande
        public int ProduitID { get; set; }

        // Nom lisible du produit (utile pour les retours d'API, affichage côté client, etc.)
        public string NomProduit { get; set; }

        // Statut actuel de la commande (par défaut à 'EnAttenteLivraison')
        public StatutCommande Statut { get; set; } = StatutCommande.EnAttenteLivraison;

        // Nom du fournisseur chez qui la commande a été passée
        public string FournisseurNom { get; set; }
    }
}