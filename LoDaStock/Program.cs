// Importation des espaces de noms nécessaires
using System.Diagnostics;
using LoDaStock.Models;                       // Référence aux modèles de l'application comme le DbContext personnalisé
using Microsoft.EntityFrameworkCore;          // Pour utiliser Entity Framework Core et la connexion à la base de données

// Création du générateur d'application Web ASP.NET Core (pattern minimal hosting)
var builder = WebApplication.CreateBuilder(args);

// Configuration des services CORS pour permettre les requêtes depuis n'importe quelle origine
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()              // Autorise toutes les origines (pratique pour l'API, à sécuriser en production)
              .AllowAnyHeader()              // Autorise tous les headers HTTP
              .AllowAnyMethod()              // Autorise toutes les méthodes HTTP (GET, POST, etc.)
    );
});


// Ajoute les services pour utiliser les contrôleurs MVC et configure la sérialisation JSON
builder.Services.AddControllersWithViews()
    .AddJsonOptions(opt =>
    {
        // Ignore les cycles de référence dans les objets JSON, évite les erreurs de sérialisation
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        // Active l'indentation du JSON pour la lisibilité
        opt.JsonSerializerOptions.WriteIndented = true;
    });

// Configuration du contexte de base de données avec Entity Framework Core et SQL Server
builder.Services.AddDbContext<LoDaStockDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();   // Ajoute un explorateur d'API pour Swagger
builder.Services.AddSwaggerGen();            // Génère la documentation Swagger pour l'API

var app = builder.Build();                   // Création de l'instance de l'application

// Configuration du pipeline de gestion des requêtes HTTP
if (!app.Environment.IsDevelopment())
{
    // Utilise une page d'erreur personnalisée en production
    app.UseExceptionHandler("/Home/Error");
    // Active HSTS (HTTP Strict Transport Security) pour renforcer la sécurité
    // La valeur par défaut est 30 jours, peut être modifiée selon les besoins
    app.UseHsts();
}

// Redirige les requêtes HTTP vers HTTPS
//app.UseHttpsRedirection();

// Permet l'accès aux fichiers statiques (CSS, JS, images, etc.)
app.UseStaticFiles();

// Ajoute le middleware de routage, permet de déterminer quel code doit traiter la requête
app.UseRouting();

// Active la politique CORS configurée plus haut
app.UseCors();

// Active les middlewares Swagger et SwaggerUI pour la documentation interactive de l'API
app.UseSwagger();
app.UseSwaggerUI();

// Gère l'autorisation (authentification non implémentée par défaut ici)
app.UseAuthorization();

// Définit la route par défaut des contrôleurs MVC (par exemple HomeController avec action Index)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Re-définit la politique CORS juste avant la fin de la configuration
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod());

// Ouvre automatiquement la page Accueil.html dans le navigateur
var url = "http://localhost:5000/Accueil.html";
try
{
    Process.Start(new ProcessStartInfo
    {
        FileName = url,
        UseShellExecute = true
    });
}
catch { /* Ignore les erreurs d'ouverture de navigateur */ }

// Démarre l'application ASP.NET Core
app.Run();