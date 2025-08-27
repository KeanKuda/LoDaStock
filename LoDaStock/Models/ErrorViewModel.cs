namespace LoDaStock.Models
{
    // ViewModel utilisé pour l'affichage des erreurs (généralement dans les pages d'erreur)
    public class ErrorViewModel
    {
        // L'identifiant unique de la demande ayant généré l'erreur (souvent utilisé pour le support/debug)
        public string? RequestId { get; set; }

        // Propriété calculée pour savoir si un RequestId doit être affiché
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}