// Adresse de l'API pour interagir avec les fournisseurs
const API_FOURNISSEUR = "/api/Fournisseur";
// Stocke localement la liste des fournisseurs récupérés
let fournisseurs = [];

// Fonction utilitaire d'affichage de message (ex : alert basique)
// Peut être remplacée par un système de notifications (toast par exemple)
function afficherMessage(msg, isErreur = false) {
    // Affiche une boîte de dialogue "alert" pour informer l'utilisateur
    alert(msg);
}

// Affiche les fournisseurs dans le tableau HTML
async function afficherFournisseurs() {
    // Récupère la liste des fournisseurs via l'API (GET)
    const res = await fetch(API_FOURNISSEUR);
    const data = await res.json();
    fournisseurs = data; // Stockage global pour usage modale
    let rows = "";
    // Affiche les fournisseurs en partant du plus récent (reverse)
    data.reverse().forEach(f => {
        rows += `<tr>
            <td>${f.fournisseurID}</td>
            <td>${f.nomFournisseur}</td>
            <td>${f.coordonneesFournisseur}</td>
            <td>${f.adresseMail}</td>
            <td>${f.statutFour ? "Actif" : "Inactif"}</td>
            <td>
                <!-- Boutons Suppression/Édition (icônes Bootstrap) -->
                <button class="btn btn-danger btn-sm" onclick="supprimerFournisseur(${f.fournisseurID})"><i class="bi bi-trash"></i></button>
                <button class="btn btn-warning btn-sm" onclick="afficherEditionFournisseur(${f.fournisseurID})"><i class="bi bi-pencil"></i></button>
            </td>
        </tr>`;
    });
    // On insère les lignes construites dans le <tbody> du tableau HTML
    document.querySelector("#fournisseursTable tbody").innerHTML = rows;
}

// ---------------------------
// Ajout d'un fournisseur (formulaire d'ajout)
// ---------------------------
document.getElementById("fournisseurForm").onsubmit = async (e) => {
    e.preventDefault(); // Empêche le rechargement de la page
    const fournisseur = {
        nomFournisseur: document.getElementById("nouveauNomFournisseur").value,
        coordonneesFournisseur: document.getElementById("nouveauCoordonneesFournisseur").value,
        adresseMail: document.getElementById("nouveauAdresseMail").value
    };
    // Envoi via POST à l'API fournisseur
    const res = await fetch(API_FOURNISSEUR, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(fournisseur)
    });
    if (res.ok) {
        // Si tout va bien, notification + maj du tableau + reset form
        afficherMessage("Fournisseur ajouté !");
        afficherFournisseurs();
        e.target.reset();
    } else {
        afficherMessage("Erreur lors de l'ajout.", true);
    }
};

// ---------------------------
// Suppression d'un fournisseur (bouton "poubelle")
// ---------------------------
window.supprimerFournisseur = async function (id) {
    // Confirmation avant suppression
    if (!confirm("Supprimer ce fournisseur ?")) return;
    // Envoie la requête DELETE à l'API
    const res = await fetch(`${API_FOURNISSEUR}/${id}`, { method: "DELETE" });
    if (res.ok) {
        afficherMessage("Fournisseur supprimé !");
        afficherFournisseurs(); // Rafraîchit la liste affichée
    } else {
        afficherMessage("Erreur suppression.", true);
    }
};

// ---------------------------
// Ouvre la modale d'édition d'un fournisseur sélectionné
// Charge les données du fournisseur dans la modale (formulaire de modification)
// ---------------------------
window.afficherEditionFournisseur = function (id) {
    // Recherche du fournisseur à éditer dans la liste courante
    const f = fournisseurs.find(x => x.fournisseurID === id);
    if (!f) return alert("Fournisseur introuvable !");
    // Prémplit les champs dans la modale avec les valeurs du fournisseur sélectionné
    document.getElementById("editFournisseurID").value = f.fournisseurID;
    document.getElementById("editNomFournisseur").value = f.nomFournisseur;
    document.getElementById("editCoordonneesFournisseur").value = f.coordonneesFournisseur;
    document.getElementById("editAdresseMail").value = f.adresseMail || "";

    // Affiche le statut actuel et le bouton d'activation/désactivation
    document.getElementById("editStatutText").textContent = "Statut actuel : " + (f.statutFour ? "Actif" : "Inactif");
    const btn = document.getElementById("btnToggleStatut");
    btn.textContent = f.statutFour ? "Désactiver" : "Activer";
    btn.onclick = function () {
        changerStatutModal(f.fournisseurID, !f.statutFour);
    };

    // Affiche la modale (style display:flex)
    document.getElementById("modalEditFournisseur").style.display = "flex";
}

// Ferme la modale d'édition sans sauvegarder
window.fermerModalFournisseur = function () {
    document.getElementById("modalEditFournisseur").style.display = "none";
}

// ---------------------------
// Validation/sauvegarde des modifications sur un fournisseur (PUT)
// Lance la requête PUT à l'API avec les données du formulaire édition
// ---------------------------
window.sauverModificationFournisseur = async function (e) {
    e.preventDefault();
    const id = Number(document.getElementById("editFournisseurID").value);
    // Lit le statut actuel affiché dans la modale (texte "Actif" ou pas)
    const statutActuel = document.getElementById("editStatutText").textContent.includes("Actif");
    const fournisseur = {
        fournisseurID: id,
        nomFournisseur: document.getElementById("editNomFournisseur").value,
        coordonneesFournisseur: document.getElementById("editCoordonneesFournisseur").value,
        adresseMail: document.getElementById("editAdresseMail").value,
        statutFour: statutActuel
    };
    // Envoie la requête PUT à l'API pour modifier le fournisseur
    const res = await fetch(`${API_FOURNISSEUR}/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(fournisseur)
    });
    if (res.ok) {
        fermerModalFournisseur();
        afficherMessage("Fournisseur modifié !");
        afficherFournisseurs();
    } else {
        afficherMessage("Erreur lors de la modification", true);
    }
}

// ---------------------------
// Change le statut d'un fournisseur (Actif/Inactif) dans la modale d'édition uniquement
// Cette route API doit exister côté serveur
// ---------------------------
function changerStatutModal(id, nouveauStatut) {
    fetch(API_FOURNISSEUR + `/${id}/statut`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(nouveauStatut)
    })
        .then(res => {
            if (res.ok) {
                // Mets à jour le fournisseur localement dans le tableau JS pour gardes la synchro modale/liste
                const f = fournisseurs.find(x => x.fournisseurID === id);
                if (f) f.statutFour = nouveauStatut;

                // Met à jour l'affichage du statut et du bouton dans la modale ouverte
                document.getElementById("editStatutText").textContent = "Statut actuel : " + (nouveauStatut ? "Actif" : "Inactif");
                document.getElementById("btnToggleStatut").textContent = nouveauStatut ? "Désactiver" : "Activer";
            } else {
                alert("Erreur lors du changement de statut !");
            }
        });
}

// ---------------------------
// Initialisation de la page : affiche directement les fournisseurs au chargement
// ---------------------------
afficherFournisseurs();