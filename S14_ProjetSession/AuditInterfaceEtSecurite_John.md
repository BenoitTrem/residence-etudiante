Audit de l'interface et revue de code et de sécurité — John Zuleta et aide(Claude:affichage)

Cet audit couvre les pages et contrôleurs liés à la gestion des campus, des étudiants et des programmes : 
- Campus/Index (vue Index.cshtml) 
- Campus/Creer (vue Creer.cshtml) 
- Campus/Modifier (vue Modifier.cshtml) 
- Etudiant/Index (vue Index.cshtml) 
- Etudiant/Creer (vue Creer.cshtml)
- Etudiant/Modifier (vue Modifier.cshtml)
- Programme/Index (vue Index.cshtml) 
- Programme/Creer (vue Creer.cshtml)
- Programme/Modifier (vue Modifier.cshtml)
- Tests unitaires : CampusControllerTests, EtudiantControllerTests, ProgrammeControllerTests
- Tests d'intégration : AjoutCampusTests, ModifierCampusTests, SupprimerCampusTests, AjoutEtudiantTests, ModifierEtudiantTests, SupprimerEtudiantTests, AjoutProgrammeTests, ModifierProgrammeTests, SupprimerProgrammeTests

---
# Audit de l'interface

| Catégorie                             | Aspect à vérifier                                                                      | État / Validation                                                                                                                                                                                                                     | Priorité/impact |
| ------------------------------------- | -------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------- |
| **Authentification**                  | Respect du tableau des droits d'accès dans l'interface                                 | Conforme — Les fonctionnalités visibles dans l’interface varient selon le rôle de l’utilisateur (`Admin`, `Gestionnaire`, `Étudiant`, non connecté). Les actions interdites sont masquées ou désactivées avec message explicatif.     | Élevé           |
|                                       | Respect du tableau des droits d'accès dans les requêtes HTTP                           | Conforme — Les contrôleurs utilisent `[Authorize]`, `[AllowAnonymous]` et les politiques `AdminOuGestionnaire` / `AdminUniquement` pour protéger les routes et actions sensibles. Les tests d’intégration valident les refus d’accès. | Élevé           |
| **Injection SQL**                     | Requêtes à la base de données                                                          | Conforme — Les accès aux données passent par Entity Framework Core et des requêtes LINQ typées. Aucune requête SQL construite manuellement n’a été observée.                                                                          | Élevé           |
| **Téléversement**                     | Validation du type et de la taille                                                     | Non applicable — Les fonctionnalités développées ne permettent aucun téléversement de fichier.                                                                                                                                        | Faible          |
|                                       | Nom du fichier et chemin d'accès                                                       | Non applicable — Aucun traitement de fichiers ou stockage de documents n’est utilisé dans le projet audité.                                                                                                                           | Faible          |
| **CSRF**                              | Utilisation du jeton (*AntiForgeryToken*) pour les formulaires et requêtes côté client | Conforme — Tous les formulaires POST utilisent `[ValidateAntiForgeryToken]` et les formulaires Razor génèrent automatiquement le jeton AntiForgery. Les tests d’intégration transmettent également le token lors des requêtes POST.   | Élevé           |
| **Respect des normes et conventions** | Typage, noms significatifs, documentation, etc.                                        | Conforme — Le projet respecte les conventions ASP.NET Core : noms explicites, ViewModels typés, séparation des responsabilités (contrôleurs, services, repositories) et structure cohérente du code.                                  | Moyen           |



---
# Audit de l'interface

| Aspect                                | Critère                                                                  | État / Validation                                                                                                                | Endroits à corriger | Billets associés |
| ------------------------------------- | ------------------------------------------------------------------------ | -------------------------------------------------------------------------------------------------------------------------------- | ------------------- | ---------------- |
| **Navigation**                        | Les liens fonctionnent                                                   | Conforme — Les routes vers campus, étudiants et programmes sont fonctionnelles et couvertes par des tests de routage.            | Aucun               | —                |
|                                       | L'internaute a un moyen de savoir où il est dans le site                 | Conforme — Les titres de pages (`ViewData["Title"]`) et en-têtes sont cohérents avec la section visitée .                        | Aucun               | —                |
|                                       | L'internaute a toujours une vue globale des différentes sections du site | Conforme — Le menu principal expose les sections selon les droits de l’utilisateur connecté.                                     | Aucun               | —                |
|                                       | La navigation est identique partout sur le site                          | Conforme — Le layout partagé est utilisé dans toutes les vues.                                                                   | Aucun               | —                |
|                                       | Rubriques cohérentes (organisation de l'application)                     | Conforme — Les fonctionnalités sont regroupées par domaine (`Campus`, `Etudiant`, `Programme`).                                  | Aucun               | —                |
|                                       | 8 sous-rubriques maximum (largeur de la navigation)                      | Conforme — La navigation demeure concise et limitée aux sections essentielles.                                                   | Aucun               | —                |
|                                       | 3 niveaux maximum                                                        | Conforme — L’application respecte une profondeur de navigation simple.                                           | Aucun               | —                |
| **Structure**                         | Structure sémantique logique (ex. h1, h2, h3)                            | Conforme — Les vues utilisent une hiérarchie cohérente des titres et sections.                                                   | Aucun               | —                |
| **Typographie**                       | La taille du texte reflète la hiérarchie de l'information                | Conforme — Les titres, formulaires et tableaux respectent une hiérarchie visuelle claire.                                        | Aucun               | —                |
|                                       | Les polices sont choisies avec soin et utilisées de façon cohérente      | Conforme — Les styles Bootstrap et personnalisés sont appliqués uniformément.                                                    | Aucun               | —                |
|                                       | Largeur des paragraphes                                                  | Conforme — Les contenus restent lisibles et bien espacés dans les formulaires et cartes d’information.                           | Aucun               | —                |
| **Couleurs**                          | 5 couleurs maximum                                                       | Conforme — Palette limitée et cohérente avec Bootstrap et les couleurs du projet.                                                | Aucun               | —                |
|                                       | Les couleurs ne sont pas la seule source de rétroaction                  | Conforme — Les états utilisent également des icônes, messages textuels et boutons désactivés.                                    | Aucun               | —                |
|                                       | Les contrastes sont conformes aux normes établies (WCAG)                 | Conforme — Les contrastes des textes et boutons restent lisibles dans les vues principales.                                      | Aucun               | —                |
| **Formulaires**                       | Toutes les données sont validées                                         | Conforme — Validation côté client et serveur (`ModelState`, annotations, validations métier).                                    | Aucun               | —                |
|                                       | Les utilisatrices et les utilisateurs sont informés des restrictions     | Conforme — Les formats attendus sont affichés dans l’interface.                                                                  | Aucun               | —                |
|                                       | Les entrées obligatoires sont clairement indiquées                       | Conforme — Les champs obligatoires affichent des validations et messages d’erreur près des champs concernés.                     | Aucun               | —                |
| **Guidage et traitement des erreurs** | Assistance à l'utilisation et prévention des erreurs                     | Conforme — Info-bulles, validations de doublons et messages explicatifs présents dans les formulaires.                           | Aucun               | —                |
|                                       | Retour aux actions de l'internaute                                       | Conforme — Les opérations affichent des confirmations via `TempData` (succès, erreurs).                                          | Aucun               | —                |
|                                       | Les actions destructrices nécessitent une confirmation                   | Conforme — Les suppressions utilisent une modale `<dialog class="danger">`.                                                      | Aucun               | —                |
|                                       | Gestion claire des erreurs                                               | Conforme — Les erreurs sont affichées directement près des champs concernés et les cas serveur redirigent vers une vue `Erreur`. | Aucun               | —                |
| **Homogénéité**                       | Couleurs, icônes et polices cohérentes                                   | Conforme — Uniformité visuelle entre les différentes pages de gestion.                                                           | Aucun               | —                |
|                                       | Composantes uniformes                                                    | Conforme — Les boutons « Confirmer » / « Annuler » gardent le même comportement et positionnement.                               | Aucun               | —                |
|                                       | Vocabulaire uniforme                                                     | Conforme — Les termes utilisés (`Campus`, `Étudiant`, `Programme`) sont constants dans toute l’application.                      | Aucun               | —                |
| **Réactivité**                        | Adaptation au format d’affichage                                         | Conforme — Les pages utilisent Bootstrap et demeurent utilisables sur différentes résolutions.                                   | Aucun               | —                |
| **Heuristiques de Jakob**             | Vocabulaire conforme au monde réel                                       | Conforme — Les termes et actions correspondent au domaine de la gestion étudiante.                                               | Aucun               | —                |
|                                       | Possibilité d'annuler une action                                         | Conforme — Les modales et formulaires permettent l’annulation avant confirmation.                                                | Aucun               | —                |
|                                       | Diminution de la charge cognitive                                        | Conforme — Les actions importantes sont visibles et les formulaires affichent uniquement les champs pertinents.                  | Aucun               | —                |
|                                       | Esthétisme et minimalisme                                                | Conforme — Les vues évitent les informations inutiles et privilégient la lisibilité.                                             | Aucun               | —                |

---
## Conclusion 
L'ensemble des fonctionnalités que j'ai développées (gestion des campus, des étudiants et des programmes) est couvert par des tests unitaires et des tests d'intégration. 
J'ai porté une attention particulière à la sécurité : les pages publiques sont accessibles sans connexion, les actions d'écriture exigent un rôle gestionnaire ou administrateur, 
et les suppressions sont réservées aux administrateurs uniquement. Les interfaces ont également été conçues pour refléter ces restrictions directement dans l'UI, notamment via
les boutons désactivés avec tooltip pour les gestionnaires.

Autres commentaires:J'ai realiser trop tard et après avoir accorder entre collègues d'arrêter push , que je n'avais pas completement fini le tri, filtre et pagination ce qui pourrait nuire
l'interface en termes de navigation et affichage.
