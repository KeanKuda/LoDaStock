namespace LoDaStock.Models
{
    using Microsoft.EntityFrameworkCore;

    // Le contexte principal de la base de données pour ton application de gestion de stock
    public class LoDaStockDbContext : DbContext
    {
        public LoDaStockDbContext(DbContextOptions<LoDaStockDbContext> options) : base(options) { }

        // Déclaration des DbSet correspondant à chaque entité/table
        public DbSet<Commande> Commandes { get; set; }
        public DbSet<Produit> Produits { get; set; }
        public DbSet<Fournisseur> Fournisseurs { get; set; }
        public DbSet<SortieStock> SortiesStock { get; set; }
        public DbSet<ProduitFournisseur> ProduitFournisseurs { get; set; }

        // Configuration des relations et des tables lors de la création du modèle
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Mapping explicite des tables (optionnel mais bon pour la clarté)
            modelBuilder.Entity<Commande>().ToTable("Commandes");
            modelBuilder.Entity<Produit>().ToTable("Produits");
            modelBuilder.Entity<Fournisseur>().ToTable("Fournisseurs");
            modelBuilder.Entity<SortieStock>().ToTable("SortiesStock");
            modelBuilder.Entity<ProduitFournisseur>().ToTable("ProduitFournisseur");

            // Définition de la clé primaire composite pour la table de jointure many-to-many
            modelBuilder.Entity<ProduitFournisseur>()
                .HasKey(pf => new { pf.ProduitID, pf.FournisseurID });

            // Configuration des relations vers Produit
            modelBuilder.Entity<ProduitFournisseur>()
                .HasOne(pf => pf.Produit)
                .WithMany(p => p.ProduitFournisseurs)
                .HasForeignKey(pf => pf.ProduitID);

            // Configuration des relations vers Fournisseur
            modelBuilder.Entity<ProduitFournisseur>()
                .HasOne(pf => pf.Fournisseur)
                .WithMany(f => f.ProduitFournisseurs)
                .HasForeignKey(pf => pf.FournisseurID);
        }
    }
}