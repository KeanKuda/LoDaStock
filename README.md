Annexe : Documentation de déploiement – LoDaStock
1. Pré-requis

      Système d’exploitation : Windows 10 ou supérieur
      
      Logiciels nécessaires :
      
      Visual Studio 2022 ou version récente avec charge de travail ASP.NET et développement web
      
      .NET 7 SDK
      
      SQL Server 2022 ou version compatible (Express ou Developer)
      
      Git pour le clonage du dépôt
      
      Accès utilisateur : droits d’administrateur pour l’installation et la configuration de SQL Server

2. Installation de l’application

      - Cloner le dépôt GitHub
      
      git clone https://github.com/KeanKuda/ECF.git
      cd ECF/LoDaStock
      
      - Restaurer les packages NuGet
        
      Ouvrir le projet dans Visual Studio → clic droit sur la solution → Restore NuGet Packages.
      
      - Compilation
      
      Dans Visual Studio : clic droit sur la solution → Build Solution
      Ligne de commande :
      
      dotnet build

3. Configuration de SQL Server

      - Créer une base de données
      
      Ouvrir SQL Server Management Studio (SSMS)
      
      Clic droit sur Databases → New Database → nommer la base : LoDaStockDB
      
      - Exécuter les scripts SQL
      
      Scripts disponibles dans le dossier /Scripts du projet :
      
      CreateTables.sql : création des tables
      
      SeedData.sql : insertion des données initiales
      
      - Configurer la chaîne de connexion
      Modifier le fichier appsettings.json :
      
      "ConnectionStrings": {
          "DefaultConnection": "Server=localhost\\SQLSERV22;Database=LoDaStockDB;Integrated Security=True;TrustServerCertificate=True;"
      }

4. Lancement de l’application

      Via Visual Studio : sélectionner le projet LoDaStock comme projet de démarrage → clic sur Start Debugging (F5)
      
      Via ligne de commande :
      
      dotnet run --project LoDaStock
      
      
      L’application sera accessible à l’adresse : https://localhost:5001
