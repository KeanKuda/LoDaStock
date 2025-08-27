namespace LoDaStock.Data
{
    // Classe DTO destinée à la création d'un produit via une API ou un formulaire.
    public class ProduitCreateDto
    {
        // Nom du produit à créer
        public string NomProduit { get; set; }

        // Commentaire informatif ou descriptif sur ce produit
        public string Commentaire { get; set; }

        // Prix unitaire du produit (en devise monétaire)
        public decimal PrixUnitaire { get; set; }

        // Quantité de produit disponible lors de la création
        public int QuantiteDisponible { get; set; }

        // Liste des identifiants des fournisseurs associés au produit lors de la création.
        // Permet de lier ce produit à plusieurs fournisseurs dès sa création.
        public List<int> FournisseurIDs { get; set; }
    }
}