using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoDaStock.Migrations
{
    /// <inheritdoc />
    public partial class CreationJointureProduitsFournisseurs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fournisseurs",
                columns: table => new
                {
                    FournisseurID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomFournisseur = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CoordonneesFournisseur = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdresseMail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    statutFour = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fournisseurs", x => x.FournisseurID);
                });

            migrationBuilder.CreateTable(
                name: "Produits",
                columns: table => new
                {
                    ProduitID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomProduit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Commentaire = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrixUnitaire = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    QuantiteDisponible = table.Column<int>(type: "int", nullable: false),
                    FournisseurID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produits", x => x.ProduitID);
                    table.ForeignKey(
                        name: "FK_Produits_Fournisseurs_FournisseurID",
                        column: x => x.FournisseurID,
                        principalTable: "Fournisseurs",
                        principalColumn: "FournisseurID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Commandes",
                columns: table => new
                {
                    CommandeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProduitID = table.Column<int>(type: "int", nullable: false),
                    FournisseurID = table.Column<int>(type: "int", nullable: false),
                    DateCommande = table.Column<DateTime>(type: "datetime2", nullable: false),
                    QuantiteCommandee = table.Column<int>(type: "int", nullable: false),
                    Statut = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commandes", x => x.CommandeID);
                    table.ForeignKey(
                        name: "FK_Commandes_Fournisseurs_FournisseurID",
                        column: x => x.FournisseurID,
                        principalTable: "Fournisseurs",
                        principalColumn: "FournisseurID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Commandes_Produits_ProduitID",
                        column: x => x.ProduitID,
                        principalTable: "Produits",
                        principalColumn: "ProduitID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProduitFournisseur",
                columns: table => new
                {
                    ProduitID = table.Column<int>(type: "int", nullable: false),
                    FournisseurID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProduitFournisseur", x => new { x.ProduitID, x.FournisseurID });
                    table.ForeignKey(
                        name: "FK_ProduitFournisseur_Fournisseurs_FournisseurID",
                        column: x => x.FournisseurID,
                        principalTable: "Fournisseurs",
                        principalColumn: "FournisseurID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProduitFournisseur_Produits_ProduitID",
                        column: x => x.ProduitID,
                        principalTable: "Produits",
                        principalColumn: "ProduitID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SortiesStock",
                columns: table => new
                {
                    SortieID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProduitID = table.Column<int>(type: "int", nullable: false),
                    QuantiteSortie = table.Column<int>(type: "int", nullable: false),
                    DateSortie = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Commentaire = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SortiesStock", x => x.SortieID);
                    table.ForeignKey(
                        name: "FK_SortiesStock_Produits_ProduitID",
                        column: x => x.ProduitID,
                        principalTable: "Produits",
                        principalColumn: "ProduitID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Commandes_FournisseurID",
                table: "Commandes",
                column: "FournisseurID");

            migrationBuilder.CreateIndex(
                name: "IX_Commandes_ProduitID",
                table: "Commandes",
                column: "ProduitID");

            migrationBuilder.CreateIndex(
                name: "IX_ProduitFournisseur_FournisseurID",
                table: "ProduitFournisseur",
                column: "FournisseurID");

            migrationBuilder.CreateIndex(
                name: "IX_Produits_FournisseurID",
                table: "Produits",
                column: "FournisseurID");

            migrationBuilder.CreateIndex(
                name: "IX_SortiesStock_ProduitID",
                table: "SortiesStock",
                column: "ProduitID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Commandes");

            migrationBuilder.DropTable(
                name: "ProduitFournisseur");

            migrationBuilder.DropTable(
                name: "SortiesStock");

            migrationBuilder.DropTable(
                name: "Produits");

            migrationBuilder.DropTable(
                name: "Fournisseurs");
        }
    }
}
