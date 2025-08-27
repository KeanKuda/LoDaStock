// --- Constantes des URLs de l'API ---
const API_PRODUIT = "api/Produit";
const API_FOURNISSEUR = "api/Fournisseur";

// Variables globales pour stocker produits et fournisseurs (pour réutilisation en édition)
let produits = [];
let fournisseurs = [];

// --- Petite fonction utilitaire pour afficher un message (alerte simple ici) ---
function afficherMessage(msg, isErreur = false) {
    alert(msg);
}

// ----------- CRUD PRODUIT ------------

// Affiche tous les produits 
async function afficherStock() {
    // Récupère la liste de tous les produits depuis l'API
    const res = await fetch(API_PRODUIT);
    const data = await res.json();
    produits = data;

    let byProduit = {};
    // Construction d'un dictionnaire par ID produit pour grouper les fournisseurs
    data.forEach(prod => {
        const id = prod.produitID;
        if (!byProduit[id]) {
            byProduit[id] = {
                produitID: prod.produitID,
                nomProduit: prod.nomProduit,
                commentaire: prod.commentaire,
                quantiteDisponible: prod.quantiteDisponible
            };
        }
    });

    // Génère le HTML (tous les fournisseurs d'un produit sur la même ligne)
    let rows = "";
    Object.values(byProduit).reverse().forEach(prod => {
        // Affiche la liste de fournisseurs (unique) pour chaque produit
        let nomsFournisseurs = [...new Set(prod.fournisseurs)].join(", ");
        rows += `<tr>
            <td>${prod.produitID}</td>
            <td>${prod.nomProduit}</td>
            <td>${prod.commentaire || ""}</td>
            <td><b>${prod.quantiteDisponible ?? "?"}</b></td>
            <td>
                <!-- Boutons suppression & édition produits -->
                <button class="btn btn-danger btn-sm" onclick="supprimerProduit(${prod.produitID})"><i class="bi bi-trash"></i></button>
                <button class="btn btn-warning btn-sm" onclick="afficherEditionProduit(${prod.produitID})"><i class="bi bi-pencil"></i></button>
            </td>
        </tr>`;
    });
    // Injection du HTML généré dans le <tbody> du tableau principal
    document.querySelector("#stockTable tbody").innerHTML = rows;
}


// --- AJOUT : formulaire d'ajout produit ---
document.getElementById("produitForm").onsubmit = async function (e) {
    e.preventDefault();
    const form = e.target;

    // Récupère tous les fournisseurs sélectionnés (tableau d’IDs)
    const select = document.getElementById("selectFournisseurs");
    const fournisseurIDs = Array.from(select.selectedOptions).map(opt => Number(opt.value));

    // Création de l'objet produit à envoyer en POST au backend
    const produit = {
        NomProduit: document.getElementById("nouveauNomProduit").value,
        Commentaire: document.getElementById("nouvelleDescription").value,
        FournisseurIDs: fournisseurIDs  // <-- Important ! Doit correspondre à l'attendu côté C#
    };

    const res = await fetch(API_PRODUIT, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(produit)
    });
    if (res.ok) {
        afficherMessage("Produit ajouté !");
        afficherStock();
        form.reset();
        select.selectedIndex = -1; // vide la sélection fournisseurs
    } else {
        afficherMessage("Erreur lors de l'ajout.", true);
    }
};

// --- SUPPRESSION PRODUIT ---
window.supprimerProduit = async function (id) {
    if (!confirm("Supprimer ce produit ?")) return;
    // Envoie la requête DELETE au serveur pour le produit concerné
    const res = await fetch(`${API_PRODUIT}/${id}`, { method: "DELETE" });
    if (res.ok) {
        afficherMessage("Produit supprimé !");
        afficherStock();
    } else {
        afficherMessage("Erreur suppression.", true);
    }
};

// --- Ouvre la fenêtre/modale d'édition produit ---
window.afficherEditionProduit = async function (id) {
    // Récupère le produit concerné dans le tableau JS global
    const p = produits.find(x => x.produitID === id);
    if (!p) return alert("Produit introuvable !");
    document.getElementById("editProduitID").value = p.produitID;
    document.getElementById("editNomProduit").value = p.nomProduit;
    document.getElementById("editDescription").value = p.commentaire || "";

    // Récupère fournisseurs S'IL N'Y EN A PAS DÉJÀ
    if (!fournisseurs || !fournisseurs.length) {
        const res = await fetch(API_FOURNISSEUR);
        fournisseurs = await res.json();
    }

    // Récupère les IDs des fournisseurs du produit
    const fournisseurIDsProduit = (p.produitFournisseurs || []).map(x => x.fournisseurID);

    // Remplissage du select pour multi-sélection fournisseurs
    const select = document.getElementById("editFournisseurs");
    select.innerHTML = "";
    fournisseurs.forEach(f => {
        const opt = document.createElement("option");
        opt.value = f.fournisseurID;
        opt.text = f.nomFournisseur;
        if (fournisseurIDsProduit.includes(f.fournisseurID)) opt.selected = true;
        select.appendChild(opt);
    });

    // Affiche la modale pop-in
    document.getElementById("modalEditProduit").style.display = "flex";
    document.getElementById("formEditProduit").onsubmit = sauverModificationProduit;
}

// --- Ferme la modale édition ---
window.fermerModalProduit = function () {
    document.getElementById("modalEditProduit").style.display = "none";
};

// --- Sauvegarde les modifications faites sur un produit (EDIT) ---
async function sauverModificationProduit(e) {
    e.preventDefault();
    const id = Number(document.getElementById("editProduitID").value);

    // Récupère la multi-sélection des fournisseurs
    const select = document.getElementById("editFournisseurs");
    const fournisseurIDs = Array.from(select.selectedOptions).map(x => Number(x.value));

    const produit = {
        ProduitID: id,
        NomProduit: document.getElementById("editNomProduit").value,
        Commentaire: document.getElementById("editDescription").value,
        FournisseurIDs: fournisseurIDs // <--- AJOUT ICI : transmet tes IDs !
    };

    // Envoi la requête PUT à l'API pour sauvegarder la modification du produit
    const res = await fetch(`${API_PRODUIT}/${id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(produit)
    });
    if (res.ok) {
        afficherMessage("Produit modifié !");
        afficherStock();
        fermerModalProduit();
    } else {
        afficherMessage("Erreur lors de la modification", true);
    }
}

// Ouvre la fenêtre de sortie de stock pour enregistrer un retrait
function ouvrirSortieStock() {
    // Rempli la liste des produits pour le select
    const select = document.getElementById('ss_produit');
    select.innerHTML = "";
    produits.forEach(prod => {
        select.innerHTML += `<option value="${prod.produitID}">${prod.nomProduit}</option>`;
    });
    // Affiche la dispo du premier produit actif
    majStockDisponible();

    document.getElementById('ss_quantite').value = '';
    document.getElementById('ss_commentaire').value = '';
    document.getElementById('modalSortieStock').style.display = 'block';
}

// Ferme la fenêtre/modale de sortie de stock
function fermerSortieStock() {
    document.getElementById('modalSortieStock').style.display = 'none';
}

// Met à jour l'affichage de la quantité dispo selon produit sélectionné
function majStockDisponible() {
    let prodId = document.getElementById('ss_produit').value;
    let prod = produits.find(p => p.produitID == prodId);
    document.getElementById('ss_dispoActuel').textContent = prod ? prod.quantiteDisponible : '-';
    document.getElementById('ss_quantite').max = prod ? prod.quantiteDisponible : 1;
}

// Enregistre la sortie de stock (décrément du stock produit), appelle API/SortieStock
async function enregistrerSortieStock(event) {
    event.preventDefault();
    const produitID = parseInt(document.getElementById('ss_produit').value);
    const quantiteSortie = Number(document.getElementById('ss_quantite').value);
    const commentaire = document.getElementById('ss_commentaire').value;

    let body = {
        produitID,
        quantiteSortie,
        commentaire
    };

    try {
        let response = await fetch("/api/SortieStock", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(body)
        });

        if (!response.ok) {
            let msg = await response.text();
            throw new Error(msg || "Erreur API");
        }

        fermerSortieStock();
        await afficherStock();
        alert("Sortie de stock enregistrée !");
    } catch (err) {
        alert("Erreur sortie stock : " + err.message);
    }
}

// Remplit la liste déroulante fournisseurs dans le formulaire d’ajout produit
async function remplirSelectFournisseurs() {
    const res = await fetch(API_FOURNISSEUR);
    const data = await res.json();
    const select = document.getElementById("selectFournisseurs");
    select.innerHTML = data.map(f => `<option value="${f.fournisseurID}">${f.nomFournisseur}</option>`).join("");
}

// --- INITIALISATION AU CHARGEMENT DU DOM ---
window.addEventListener("DOMContentLoaded", () => {
    afficherStock();
    remplirSelectFournisseurs();
});