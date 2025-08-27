namespace LoDaStock.Models
{
    // Enumération représentant les différents statuts possibles d'une commande
    public enum StatutCommande
    {
        // Commande en attente de livraison
        EnAttenteLivraison = 0,

        // Commande en attente de validation
        EnAttenteValidation = 1,

        // Commande validée
        Validee = 2
    }

    // Classe représentant une commande dans le modèle de données principal
    public class Commande
    {
        // Identifiant unique de la commande
        public int CommandeID { get; set; }

        // Identifiant du produit concerné par la commande
        public int ProduitID { get; set; }

        // Référence vers l'objet Produit associé (peut être nulle)
        public Produit? Produit { get; set; }

        // Identifiant du fournisseur (nullable : une commande peut ne pas avoir de fournisseur associé)
        public int? FournisseurID { get; set; }

        // Référence vers l'objet Fournisseur associé (peut être nulle)
        public Fournisseur? Fournisseur { get; set; }

        // Date à laquelle la commande a été passée
        public DateTime DateCommande { get; set; }

        // Quantité commandée pour cette commande
        public int QuantiteCommandee { get; set; }

        // Statut actuel de la commande (par défaut à 'EnAttenteLivraison')
        public StatutCommande Statut { get; set; } = StatutCommande.EnAttenteLivraison;
    }
}