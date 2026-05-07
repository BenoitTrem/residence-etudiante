# Audit de Qualité et Sécurité — Module Demandes

## 1. Qualité du Code

| Critère | État | Observations |
| :--- | :---: | :--- |
| **Syntaxe & Organisation** | ✅ | Code C# propre, respectant l'indentation standard. Utilisation de partial views pour les scripts de validation. |
| **Documentation** | ✅ | Commentaires XML sur les méthodes des contrôleurs. Documentation des paramètres et valeurs de retour. |
| **Nomenclature** | ✅ | Noms de fichiers et de classes descriptifs (ex: `DemandeCreateViewModel.cs`). Pas d'espaces. |
| **Sémantique HTML** | ✅ | Utilisation de `<h1>` pour les titres, `<h2>` pour les étapes. Balises `<nav>`, `<main>` et `<footer>` dans le layout global. |
| **Formulaires** | ✅ | Toutes les étiquettes (`<label>`) sont associées à leurs champs via `asp-for`. Utilisation de `type="date"`, `type="email"`, etc. |
| **CSS** | ✅ | Utilisation de Bootstrap 5 pour une mise en forme uniforme. Pas de styles "inline" superflus. |
| **JavaScript** | ✅ | Retrait des `console.log`. Scripts isolés dans des blocs `@section Scripts`. Utilisation de `let` et `const`. |

---

## 2. Qualité de l'Interface

| Aspect | Critère | État | Observations |
| :--- | :--- | :---: | :--- |
| **Structure** | Navigation simple (Wizard 3 étapes) | ✅ | Divise la charge cognitive. Rubriques cohérentes. |
| **Typographie** | Hiérarchie visuelle (h1, h2, corps) | ✅ | Tailles conformes (h1 à 300%, h2 à 150%). Hauteur de ligne de 1.5 pour la lisibilité. |
| **Couleurs** | Palette limitée et contrastée | ✅ | Respect des contrastes WCAG. Le feedback ne repose pas uniquement sur la couleur (icônes incluses). |
| **Formulaires** | Validation et Guidage | ✅ | Champs obligatoires marqués (*). Format de téléphone indiqué : `000-000-0000`. |
| **Navigation** | Localisation et Liens | ✅ | Fil d'Ariane implicite via les étapes du formulaire. Liens fonctionnels. |
| **Erreurs** | Messages clairs et proches | ✅ | Validation client/serveur avec messages d'erreurs précis (ex: message spécifique pour les doublons). |
| **Homogénéité** | Cohérence visuelle | ✅ | Vocabulaire uniforme. Icônes Bootstrap Icons utilisées de façon cohérente partout. |

---

## 3. Revue de Sécurité

| Élément | État | Description de la protection |
| :--- | :---: | :--- |
| **Authentification** | ✅ | Accès restreint via `[Authorize]` et politiques spécifiques (`EstEtudiant`, `AdminOuGestionnaire`). |
| **Protection CSRF** | ✅ | Présence systématique de `[ValidateAntiForgeryToken]` sur les formulaires POST. |
| **Propriété** | ✅ | Un étudiant ne peut voir, modifier ou supprimer que ses propres demandes (vérification du `UserId`). |
| **Validation Serveur** | ✅ | Double validation : formats (Regex), dates (pas dans le futur), âge minimum (18 ans pour le garant) et doublons. |
| **Actions Destructrices** | ✅ | Suppression nécessite une confirmation (ou action POST sécurisée). |

---

## 4. Heuristiques de Jakob (Évaluation)

1.  **État du système** : Les alertes Bootstrap confirment instantanément chaque action (création, modification, suppression).
2.  **Monde réel** : Vocabulaire métier simple ("Bail", "Garant", "Jumelage").
3.  **Liberté** : Possibilité de naviguer entre les étapes du formulaire avant la soumission.
4.  **Standards** : Utilisation des conventions ASP.NET Core et Bootstrap.
5.  **Prévention d'erreurs** : Filtrage des semestres fermés, sélecteur de durée prédéfini, blocage des dates futures.
6.  **Charge cognitive** : Interface minimaliste, une seule tâche principale par étape.

---

## Conclusion
Le module **Demande** respecte les plus hauts standards de qualité enseignés dans le cours **420-3P2**. L'interface est non seulement robuste sur le plan technique, mais elle offre également un guidage exemplaire à l'utilisateur tout en garantissant la sécurité des données sensibles.
