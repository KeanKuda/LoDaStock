namespace LoDaStock.Models
{
    // ViewModel utilis� pour l'affichage des erreurs (g�n�ralement dans les pages d'erreur)
    public class ErrorViewModel
    {
        // L'identifiant unique de la demande ayant g�n�r� l'erreur (souvent utilis� pour le support/debug)
        public string? RequestId { get; set; }

        // Propri�t� calcul�e pour savoir si un RequestId doit �tre affich�
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}