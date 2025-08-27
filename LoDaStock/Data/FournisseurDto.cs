namespace LoDaStock.Data
{
    // Classe DTO (Data Transfer Object) utilisée pour transporter / exposer
    // les données principales d'un fournisseur, sans toute la structure du modèle complet.
    public class FournisseurDto
    {
        // Identifiant unique du fournisseur
        public int FournisseurID { get; set; }

        // Nom du fournisseur
        public string Nom { get; set; }
    }
}