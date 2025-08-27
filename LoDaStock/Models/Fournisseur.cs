namespace LoDaStock.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    // Entité représentant un fournisseur dans la base de données principale
    public class Fournisseur
    {
        // Identifiant unique du fournisseur (clé primaire)
        public int FournisseurID { get; set; }

        // Nom du fournisseur (optionnel)
        public string? NomFournisseur { get; set; }

        // Coordonnées diverses (téléphone, adresse...) du fournisseur (optionnel)
        public string? CoordonneesFournisseur { get; set; }

        // Adresse email du fournisseur (optionnel)
        public string? AdresseMail { get; set; }

        // Statut du fournisseur (par exemple, actif/inactif)
        public bool statutFour { get; set; }

        // Navigation : liste des liaisons entre Produits et Fournisseurs
        public ICollection<ProduitFournisseur> ProduitFournisseurs { get; set; }

        // Constructeur initialisant la collection pour éviter les NullReferenceException
        public Fournisseur()
        {
            ProduitFournisseurs = new HashSet<ProduitFournisseur>();
        }
    }
}