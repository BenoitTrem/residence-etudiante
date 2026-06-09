# Résidences Étudiantes

Application web de gestion des résidences étudiantes développée en ASP.NET Core MVC.

## Fonctionnalités

- Création de compte et authentification avec trois niveaux d'accès : étudiant, gestionnaire et administrateur
- Gestion du dossier étudiant (programme, campus, genre, préférences)
- Soumission et suivi de demandes de résidence
- Gestion des résidences, unités et commodités
- Traitement des demandes par les gestionnaires avec attribution d'unités
- Jumelage d'étudiants pour le partage d'unités
- Interface d'administration complète pour la gestion des utilisateurs et des référentiels

## Rôles et permissions

| Opération | Étudiant | Gestionnaire | Administrateur |
|---|:---:|:---:|:---:|
| Soumettre une demande | ✅ | ❌ | ❌ |
| Voir ses propres demandes | ✅ | ❌ | ❌ |
| Traiter les demandes | ❌ | ✅ | ✅ |
| Gérer les résidences et unités | ❌ | ✅ | ✅ |
| Attribuer une unité | ❌ | ✅ | ✅ |
| Gérer les utilisateurs | ❌ | ❌ | ✅ |
| Supprimer des données | ❌ | ❌ | ✅ |

## Stack technique

- **ASP.NET Core MVC** — architecture serveur et routage
- **Entity Framework Core** — accès aux données et migrations
- **SQL Server** — base de données relationnelle
- **Razor Views** — rendu côté serveur
- **ASP.NET Core Identity** — authentification et gestion des rôles

## Démarrage

**1. Configurer la base de données**

Dans `appsettings.json`, ajouter votre chaîne de connexion :

```json
"ConnectionStrings": {
  "ApplicationConnectionBD": "Server=VOTRE_SERVEUR;Database=VOTRE_BASE_DE_DONNEES;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

**2. Appliquer les migrations**

Dans la console du gestionnaire de package (Tools → NuGet Package Manager → Package Manager Console) :

```powershell
Update-Database
```

**3. Lancer l'application**

## License

Copyright 2026 Benoit Tremblay - Felix Lachapelle - John Sebastian Zuleta Franco. Tous droits réservés.
