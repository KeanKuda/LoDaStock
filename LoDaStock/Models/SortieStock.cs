// Déclaration du namespace contenant les modèles de l'application LoDaStock.
namespace LoDaStock.Models
{
    // Importation du namespace pour les attributs de validation de données (ex : [Key]).
    using System.ComponentModel.DataAnnotations;

    // Modèle représentant un enregistrement de sortie de stock (lorsqu'un produit sort du stock, par exemple pour une commande).
    public class SortieStock
    {
        // Spécifie que SortieID est la clé primaire de la table correspondante dans la base de données.
        [Key]
        public int SortieID { get; set; }

        // Clé étrangère pour faire le lien avec le produit concerné par la sortie de stock.
        public int ProduitID { get; set; }
        // Propriété de navigation permettant d'accéder à l'objet Produit associé à cette sortie de stock.
        public Produit Produit { get; set; }

        // Quantité de produit sortie du stock lors de cette opération.
        public int QuantiteSortie { get; set; }

        // Date et heure de la sortie de stock. Par défaut, elle prend la date et l'heure actuelles lors de la création de l'objet.
        public DateTime DateSortie { get; set; } = DateTime.Now;

        // Commentaire éventuel concernant la sortie de stock (raison, observations, etc.).
        public string Commentaire { get; set; }
    }
}