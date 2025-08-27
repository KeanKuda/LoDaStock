namespace LoDaStock.Data
{
    // Classe DTO destinée à transporter les informations lors de la sortie de stock d'un produit.
    public class SortieStockDto
    {
        // Identifiant du produit concerné par la sortie de stock
        public int ProduitID { get; set; }

        // Quantité de produit sortie du stock
        public int QuantiteSortie { get; set; }

        // Commentaire additionnel éventuellement associé à la sortie
        public string Commentaire { get; set; }
    }
}