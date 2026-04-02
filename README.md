[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/o2gylyDK)

# Projet de session — Résidences Étudiantes

## Description du projet

Application de gestion des résidences étudiantes.

---

## Tableau d'autorisations

### Référentiels (campus, programmes, genres)

| Opération              | Étudiant | Gestionnaire | Administrateur |
|------------------------|:--------:|:------------:|:--------------:|
| Voir la liste (index)  | ✅       | ✅           | ✅             |
| Voir les détails       | ✅       | ✅           | ✅             |
| Créer / modifier       | ❌       | ✅           | ✅             |
| Supprimer              | ❌       | ❌           | ✅             |

### Compte utilisateur

| Opération                    | Étudiant | Gestionnaire | Administrateur |
|------------------------------|:--------:|:------------:|:--------------:|
| Se connecter                 | ✅       | ✅           | ✅             |
| Créer son compte             | ✅       | ✅           | ✅             |
| Voir son propre compte       | ✅       | ✅           | ✅             |
| Voir les autres comptes      | ❌       | ✅           | ✅             |
| Modifier son profil          | ✅       | ✅           | ✅             |
| Gérer tous les utilisateurs  | ❌       | ❌           | ✅             |
| Supprimer son compte         | ✅       | ❌           | ✅             |
| Supprimer un autre compte    | ❌       | ❌           | ✅             |

### Dossier étudiant

| Opération                     | Étudiant | Gestionnaire | Administrateur |
|-------------------------------|:--------:|:------------:|:--------------:|
| Créer son dossier étudiant    | ✅       | ❌           | ✅             |
| Voir son propre dossier       | ✅       | ✅           | ✅             |
| Voir les dossiers des autres  | ❌       | ✅           | ✅             |
| Modifier son dossier          | ✅       | ✅           | ✅             |
| Supprimer un dossier          | ❌       | ❌           | ✅             |

### Demandes de résidence

| Opération                        | Étudiant              | Gestionnaire | Administrateur |
|----------------------------------|:---------------------:|:------------:|:--------------:|
| Créer sa demande                 | ✅                    | ❌           | ✅             |
| Voir sa propre demande           | ✅                    | ❌           | ✅             |
| Voir toutes les demandes         | ❌                    | ✅           | ✅             |
| Modifier sa demande              | ✅ (avant validation) | ❌           | ✅             |
| Soumettre sa demande             | ✅                    | ❌           | ✅             |
| Valider / refuser une demande    | ❌                    | ✅           | ✅             |
| Supprimer sa propre demande      | ✅                    | ❌           | ✅             |
| Supprimer la demande d'un autre  | ❌                    | ❌           | ✅             |

### Gestion des résidences

| Opération                       | Étudiant | Gestionnaire | Administrateur |
|---------------------------------|:--------:|:------------:|:--------------:|
| Voir les unités / résidences    | ✅       | ✅           | ✅             |
| Ajouter / modifier une unité    | ❌       | ✅           | ✅             |
| Jumeler des étudiants           | ❌       | ✅           | ✅             |
| Attribuer une unité             | ❌       | ✅           | ✅             |
| Supprimer une unité / résidence | ❌       | ❌           | ✅             |

---

## Membres de l'équipe et responsabilités

Spécifier les numéros de billets (vous pouvez compléter au fur et à mesure). Vous devrez tous déployer l'application produite.

- **Préparation de la base de données (00ST.1, 00ST.3)**
   - Felix : Modèle `Demande`, préférences de durée de bail
   - John : Modèle `Étudiant`, programme, genre,campus
   - Benoit : Modèles `Résidence`, `Unité`

- **Programmation côté serveur (00ST.1, 00ST.2, 00ST.5)**
   - Felix : Contrôleurs et logique des demandes
   - John : Contrôleurs et logique des étudiants, campus, programme et genre.
   - Benoit : Contrôleurs et lgogique des résidences et unités

- **Réalisation d'une interface utilisateur (00ST.4, 00ST.6)**
   - Felix : Formulaire de création de demande
   - John : Interface profil étudiant , gestion de erreurs
   - Benoit : Interface gestion des résidences

- **Contrôle rigoureux de la qualité de l'application (00ST.7)**
   - Felix : Tests des demandes
   - John : Tests des étudiants
   - Benoit : Tests des résidences et unités

- **Production de la documentation et respect des règles d'orthographe et de grammaire dans l'interface (00ST.2, 00ST.9)**
   - Felix : README et documentation des demandes
   - John : Documentation des modèles étudiants 
   - Benoit : Documentation des résidences et unités
