// Importation des espaces de noms n�cessaires
using System.Diagnostics;
using LoDaStock.Models;                       // R�f�rence aux mod�les de l'application comme le DbContext personnalis�
using Microsoft.EntityFrameworkCore;          // Pour utiliser Entity Framework Core et la connexion � la base de donn�es

// Cr�ation du g�n�rateur d'application Web ASP.NET Core (pattern minimal hosting)
var builder = WebApplication.CreateBuilder(args);

// Configuration des services CORS pour permettre les requ�tes depuis n'importe quelle origine
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()              // Autorise toutes les origines (pratique pour l'API, � s�curiser en production)
              .AllowAnyHeader()              // Autorise tous les headers HTTP
              .AllowAnyMethod()              // Autorise toutes les m�thodes HTTP (GET, POST, etc.)
    );
});


// Ajoute les services pour utiliser les contr�leurs MVC et configure la s�rialisation JSON
builder.Services.AddControllersWithViews()
    .AddJsonOptions(opt =>
    {
        // Ignore les cycles de r�f�rence dans les objets JSON, �vite les erreurs de s�rialisation
        opt.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        // Active l'indentation du JSON pour la lisibilit�
        opt.JsonSerializerOptions.WriteIndented = true;
    });

// Configuration du contexte de base de donn�es avec Entity Framework Core et SQL Server
builder.Services.AddDbContext<LoDaStockDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();   // Ajoute un explorateur d'API pour Swagger
builder.Services.AddSwaggerGen();            // G�n�re la documentation Swagger pour l'API

var app = builder.Build();                   // Cr�ation de l'instance de l'application

// Configuration du pipeline de gestion des requ�tes HTTP
if (!app.Environment.IsDevelopment())
{
    // Utilise une page d'erreur personnalis�e en production
    app.UseExceptionHandler("/Home/Error");
    // Active HSTS (HTTP Strict Transport Security) pour renforcer la s�curit�
    // La valeur par d�faut est 30 jours, peut �tre modifi�e selon les besoins
    app.UseHsts();
}

// Redirige les requ�tes HTTP vers HTTPS
//app.UseHttpsRedirection();

// Permet l'acc�s aux fichiers statiques (CSS, JS, images, etc.)
app.UseStaticFiles();

// Ajoute le middleware de routage, permet de d�terminer quel code doit traiter la requ�te
app.UseRouting();

// Active la politique CORS configur�e plus haut
app.UseCors();

// Active les middlewares Swagger et SwaggerUI pour la documentation interactive de l'API
app.UseSwagger();
app.UseSwaggerUI();

// G�re l'autorisation (authentification non impl�ment�e par d�faut ici)
app.UseAuthorization();

// D�finit la route par d�faut des contr�leurs MVC (par exemple HomeController avec action Index)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Re-d�finit la politique CORS juste avant la fin de la configuration
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

// D�marre l'application ASP.NET Core
app.Run();