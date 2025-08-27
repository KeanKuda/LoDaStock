// Adresse de l'API pour interagir avec les commandes
const API_URL = "/api/Commande";

// Stockage local des commandes récupérées
let commandes = [];

/**
 * Affiche un message à l'utilisateur via un popup `alert`.
 * Utilisée en fallback/alternative pour les opérations basiques.
 * @param {string} msg - Message à afficher
 * @param {boolean} isErreur - True si erreur, false sinon (non utilisé ici)
 */
function afficherMessage(msg, isErreur = false) {
    alert(msg);
}

/**
 * Affiche un message de succès ou d'erreur en haut de page.
 * Ajoute la classe correspondant au statut (success/danger) et masque le message après 2 secondes.
 * @param {string} msg  - Message à afficher
 * @param {boolean} err - True si erreur (utilise "alert-danger"), false sinon ("alert-success")
 */
function afficherMessageV2(msg, err = false) {
    const div = document.getElementById("message");
    div.className = "alert " + (err ? "alert-danger" : "alert-success");
    div.textContent = msg;
    div.classList.remove("d-none"); // Affiche le message
    setTimeout(() => div.classList.add("d-none"), 2000); // Cache au bout de 2 s
}

/**
 * Fonction pour afficher les commandes dans le tableau HTML.
 * Elle récupère la liste auprès de l'API puis met à jour le DOM.
 * Exécutée lors du chargement de la page ou après ajout/suppression/modification.
 */
async function afficherCommandes() {
    // Récupère la liste des commandes depuis l'API (GET)
    const res = await fetch(API_URL);
    const donnees = await res.json(); // les données reçues (tableau de commandes)
    commandes = donnees; // on sauvegarde le tableau globalement
    let rows = "";
    // Pour chaque commande, on construit une ligne du tableau
    donnees.reverse().forEach((c) => {
        rows += `<tr>
            <td>${c.commandeID || c.commandeId}</td>
            <td>${c.dateCommande?.substr(0, 10)}</td>
            <td>${c.nomProduit || c.nomProduit}</td>
            <td>${c.fournisseurNom || ''}</td>
            <td>${c.quantiteCommandee || c.quantitecommandee}</td>
            <td>${getStatutTexte(c.statut)}</td>
            <td>
                <!-- Bouton suppression -->
                <button class="btn btn-danger btn-sm" onclick="supprimerCommande(${c.commandeID || c.commandeId})">🗑️</button>
                <!-- Bouton édition -->
                <button class="btn btn-warning btn-sm" onclick="afficherEditionCommande(${c.commandeID || c.commandeId})">✏️</button>
            </td>
        </tr>`;
    });
    // On insère les lignes dans le tbody du tableau présent dans le HTML
    document.querySelector("#commandesTable tbody").innerHTML = rows;
}

/**
 * Gestion de l'envoi du formulaire d'ajout de commande.
 * Intercepte la soumission, envoie la commande à l'API (POST), rafraîchit l'affichage.
 */
document.getElementById("commandeForm").onsubmit = async (e) => {
    e.preventDefault(); // Empêche le rechargement de la page lors de la soumission
    const commande = {
        ProduitID: parseInt(document.getElementById("produitID").value),
        FournisseurID: parseInt(document.getElementById("fournisseurID").value),
        QuantiteCommandee: parseInt(document.getElementById("quantite").value),
        DateCommande: document.getElementById("dateCommande").value,
        Statut: parseInt(document.getElementById("statut").value)
    };
    // Envoie la nouvelle commande à l'API (requête HTTP POST)
    const res = await fetch(API_URL, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(commande),
    });
    if (res.ok) {
        // En cas de succès : on affiche un message et on rafraîchit la liste
        afficherMessage("Commande ajoutée !");
        afficherCommandes();
        e.target.reset(); // On vide le formulaire
    } else {
        // En cas d'erreur : message d'erreur
        afficherMessage("Erreur lors de l'ajout.", true);
    }
};

/**
 * Suppression d'une commande par son identifiant.
 * Fonction appelée lorsqu'on clique sur le bouton 🗑️ correspondant à une ligne du tableau.
 * Demande une confirmation, puis envoie une requête DELETE vers l'API.
 * @param {number} id - Identifiant de la commande à supprimer
 */
window.supprimerCommande = async function (id) {
    if (!confirm("Supprimer cette commande ?")) return; // Demande confirmation
    // Appel de l'API pour supprimer (méthode DELETE)
    const res = await fetch(`${API_URL}/${id}`, { method: "DELETE" });
    if (res.ok) {
        afficherMessage("Commande supprimée !");
        afficherCommandes(); // Rafraîchit la liste
    } else {
        afficherMessage("Erreur suppression.", true);
    }
};

/**
 * Ouvre une fenêtre modale pour l’édition d’une commande existante.
 * Prend l’id comme paramètre pour retrouver la commande à éditer dans le tableau JavaScript.
 * Charge aussi dynamiquement la liste des produits dans le <select> de la modale.
 * @param {number} id - Identifiant de la commande à éditer
 */
async function afficherEditionCommande(id) {
    const com = commandes.find((c) => c.commandeID === id);
    if (!com) return alert("Commande introuvable !");

    // 1. Charger d'abord les produits dans le select de la modale (et attendre que ce soit fini)
    await chargerProduitsDansSelect("editProduitID");

    // 2. Prémplir les champs de la modale avec les valeurs de la commande trouvée
    document.getElementById("editCommandeID").value = com.commandeID;
    document.getElementById("editProduitID").value = com.produitID;
    document.getElementById("editQuantiteCommandee").value = com.quantiteCommandee;
    document.getElementById("editDateCommande").value = com.dateCommande.split("T")[0];
    document.getElementById("editStatut").value = com.statut;
    // 3. Afficher la modale d'édition (affichage modal type flex)
    document.getElementById("modalEditCommande").style.display = "flex";
}

/**
 * Ferme la modale d’édition de commande.
 * Cache la fenêtre modale d'édition.
 */
function fermerModal() {
    document.getElementById("modalEditCommande").style.display = "none";
}

/**
 * Enregistre les modifications apportées à une commande (édition).
 * Envoie les nouvelles données à l’API via PUT.
 * Fonction appelée lors de la soumission du formulaire d'édition dans la modale.
 * @param {Event} e - L'événement de soumission de formulaire
 */
function sauverModification(e) {
    e.preventDefault();
    const id = Number(document.getElementById("editCommandeID").value);
    // Récupère les valeurs depuis le formulaire de modification
    const nouvelle = {
        commandeID: id,
        produitID: Number(document.getElementById("editProduitID").value),
        quantiteCommandee: Number(document.getElementById("editQuantiteCommandee").value),
        dateCommande: document.getElementById("editDateCommande").value,
        statut: Number(document.getElementById("editStatut").value)
    };
    // Appel de l'API (requête PUT pour mise à jour d'une commande spécifique)
    fetch(`${API_URL}/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(nouvelle),
    })
        .then((resp) => {
            if (!resp.ok) throw new Error("Erreur lors de la modification");
            // En cas de succès : ferme la modale et rafraîchit la liste
            afficherMessage("Commande modifié !");
            afficherCommandes();
            fermerModal();
        })
        .catch((err) => afficherMessage(err.message, true));
}

/**
 * Charge dynamiquement la liste des produits dans un <select> par appel API.
 * Idéal pour alimenter le <select> au chargement de la page ou de la modale.
 * Trie les produits par nom alphabétiquement.
 * @param {string} selectId - L'identifiant du select HTML à remplir
 */
async function chargerProduitsDansSelect(selectId) {
    const res = await fetch("/api/Produit");
    const produits = await res.json();
    // Trie alphabétique par nomProduit ou nom (compatibilité)
    produits.sort((a, b) => {
        const nomA = (a.nomProduit || a.nom).toLowerCase();
        const nomB = (b.nomProduit || b.nom).toLowerCase();
        if (nomA < nomB) return -1;
        if (nomA > nomB) return 1;
        return 0;
    });

    const select = document.getElementById(selectId);
    select.innerHTML = '<option value="">Choisissez un produit...</option>';
    produits.forEach((p) => {
        const opt = document.createElement("option");
        opt.value = p.produitID || p.produitId;
        opt.textContent = p.nomProduit || p.nom;
        select.appendChild(opt);
    });
}

/**
 * Convertit une valeur de statut de commande en texte lisible.
 * @param {number} statut - Le code du statut de la commande
 * @returns {string} Le texte du statut (ex : "En attente de livraison", "Validée", etc)
 */
function getStatutTexte(statut) {
    // Affiche le statut
    statut = Number(statut);
    switch (statut) {
        case 0: return "En attente de livraison";
        case 1: return "En attente de validation";
        case 2: return "Validée";
        default: return "Inconnu";
    }
}

// Lors du changement de produit, charger dynamiquement les fournisseurs associés à ce produit
document.getElementById('produitID').addEventListener('change', async function () {
    const produitID = this.value;
    const selectF = document.getElementById('fournisseurID');
    selectF.innerHTML = '<option value="">Chargement...</option>';
    if (!produitID) {
        selectF.innerHTML = '<option value="">Sélectionnez un produit d\'abord</option>';
        return;
    }

    // Appel API pour obtenir la liste des fournisseurs associés à CE produit
    const resp = await fetch(`/api/Produit/${produitID}/fournisseurs`);
    if (!resp.ok) {
        selectF.innerHTML = '<option value="">Erreur de chargement</option>';
        return;
    }
    const fournisseurs = await resp.json();
    if (!fournisseurs.length) {
        selectF.innerHTML = '<option value="">Aucun fournisseur disponible</option>';
        return;
    }
    selectF.innerHTML = '';
    fournisseurs.forEach(f => {
        selectF.innerHTML += `<option value="${f.fournisseurID || f.id}">${f.nom || f.nomFournisseur}</option>`;
    });
});

// S'assure que la modale d'édition a l'état classe CSS correct en supprimant/ajoutant "show"
document.getElementById("modalEditCommande").classList.add("show");
document.getElementById("modalEditCommande").classList.remove("show");

// À l’ouverture de la page, on affiche directement la liste des commandes
afficherCommandes();
// Appel au chargement de la page pour alimenter le select des produits "Ajout commande"
chargerProduitsDansSelect("produitID");