// Adresse de l'API pour interagir avec les commandes
const API_URL = "https://localhost:7006/api/Commande";

// Stockage local des commandes récupérées
let commandes = [];

/**
 * Fonction pour afficher les commandes dans le tableau HTML.
 * Elle récupère la liste auprès de l'API puis met à jour le DOM.
 */
async function afficherCommandes() {
  // Récupère la liste des commandes depuis l'API (GET)
  const res = await fetch(API_URL);
  const donnees = await res.json(); // les données reçues (tableau de commandes)
  commandes = donnees; // on sauvegarde le tableau globalement
  let rows = "";
  // Pour chaque commande, on construit une ligne (<tr>) du tableau
  donnees.forEach((c) => {
    rows += `<tr>
            <td>${c.commandeID || c.commandeId}</td>
            <td>${c.dateCommande?.substr(0, 10)}</td>
            <td>${c.nomProduit || c.nomProduit}</td>
            <td>${c.quantiteCommandee || c.quantitecommandee}</td>
            <td>
                <!-- Bouton suppression -->
                <button class="btn btn-danger btn-sm" onclick="supprimerCommande(${
                  c.commandeID || c.commandeId
                })">🗑️</button>
                <!-- Bouton édition -->
                <button class="btn btn-warning" onclick="afficherEditionCommande(${
                  c.commandeID || c.commandeId
                })">✏️</button>
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
    QuantiteCommandee: parseInt(document.getElementById("quantite").value),
    DateCommande: document.getElementById("dateCommande").value,
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
 * Fonction appelée lorsqu'on clique sur le bouton 🗑️
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
 * Affiche un message de succès ou d'erreur en haut de page.
 * Paramètre err (booléen) = message d'erreur ou non.
 */
function afficherMessage(msg, err = false) {
  const div = document.getElementById("message");
  div.className = "alert " + (err ? "alert-danger" : "alert-success");
  div.textContent = msg;
  div.classList.remove("d-none"); // Affiche le message
  setTimeout(() => div.classList.add("d-none"), 2000); // Cache au bout de 2 s
}

/**
 * Ouvre une fenêtre modale pour l’édition d’une commande existante.
 * Prend l’id comme paramètre pour retrouver la commande à éditer.
 */
async function afficherEditionCommande(id) {
  const com = commandes.find((c) => c.commandeID === id);
  if (!com) return alert("Commande introuvable !");

  // 1. Charger d'abord les produits dans le select de la modale (et attendre que ce soit fini)
  await chargerProduitsDansSelect("editProduitID");

  // 2. Prémplir les champs de la modale
  document.getElementById("editCommandeID").value = com.commandeID;
  document.getElementById("editProduitID").value = com.produitID;
  document.getElementById("editQuantiteCommandee").value =
    com.quantiteCommandee;
  document.getElementById("editDateCommande").value =
    com.dateCommande.split("T")[0];

  // 3. Afficher la modale d'édition
  document.getElementById("modalEditCommande").style.display = "flex";
}

/**
 * Ferme la modale d’édition de commande.
 */
function fermerModal() {
  document.getElementById("modalEditCommande").style.display = "none";
}

/**
 * Enregistre les modifications apportées à une commande (édition).
 * Envoie les nouvelles données à l’API via PUT.
 */
function sauverModification(e) {
  e.preventDefault();
  const id = Number(document.getElementById("editCommandeID").value);
  // Récupère les valeurs depuis le formulaire
  const nouvelle = {
    commandeID: id,
    produitID: Number(document.getElementById("editProduitID").value),
    quantiteCommandee: Number(
      document.getElementById("editQuantiteCommandee").value
    ),
    dateCommande: document.getElementById("editDateCommande").value,
  };
  // Appel de l'API (requête PUT pour mise à jour)
  fetch(`${API_URL}/${id}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(nouvelle),
  })
    .then((resp) => {
      if (!resp.ok) throw new Error("Erreur lors de la modification");
      // En cas de succès : ferme la modale et rafraîchit la liste
      fermerModal();
      afficherCommandes();
    })
    .catch((err) => afficherMessage(err.message, true));
}

async function chargerProduitsDansSelect(selectId) {
  const res = await fetch("https://localhost:7006/api/Produit");
  const produits = await res.json();
  // Trie alphabétique par nomProduit
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

document.getElementById("modalEditCommande").classList.add("show");
document.getElementById("modalEditCommande").classList.remove("show");

// À l’ouverture de la page, on affiche directement la liste des commandes
afficherCommandes();
// Appelle au chargement de la page
chargerProduitsDansSelect("produitID");
