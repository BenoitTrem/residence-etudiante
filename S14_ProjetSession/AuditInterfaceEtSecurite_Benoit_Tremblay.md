# Audit d'interface et revue de sécurité — Benoit Tremblay

## Portée

Cet audit couvre les pages et contrôleurs liés à la gestion des résidences, des unités et des commodités, ainsi que les pages Identity personnalisées :

- `Residence/Index` (vue `Residences.cshtml`)
- `Residence/ResidenceDetails`
- `Residence/AjouterResidence` (vue `AjouterResidence.cshtml`)
- `Residence/ModifierResidence` (vue `ModifierResidence.cshtml`)
- `Commodite/Index` (vue `Commodites.cshtml`)
- `Unite/Index` (vue `Unites.cshtml`)
- `Unite/AjouterUnite`
- `Unite/ModifierUnite`
- Pages Identity (connexion, inscription, gestion de compte)
- Tests unitaires : `ResidenceControllerTests`
- Tests d'intégration : `AjoutResidenceTests`, `ModifierResidenceTests`, `SupprimerResidenceTests`, `ResidenceRoutageTests`, `AjouterUniteTests`, `ModifierUniteTests`, `SupprimerUniteTests`, `UniteRoutageTests`

---

## Audit de l'interface

| Élément vérifié | État | Notes |
| --- | --- | --- |
| Navigation principale | Conforme | Les pages de résidences, unités et commodités sont accessibles selon le rôle de l'utilisateur connecté. |
| Liste des résidences | Conforme | La vue affiche les résidences avec pagination, filtres (disponibilité, nom, adresse, ville) et tri ascendant/descendant. |
| Détails d'une résidence | Conforme | La page `ResidenceDetails` affiche les informations complètes d'une résidence. |
| Ajout d'une résidence | Conforme | Le formulaire inclut une info-bulle sur le format d'adresse attendu, une info-bulle sur les commodités, et affiche un message d'alerte avec lien si aucune commodité n'est disponible. La province est en lecture seule (pré-remplie à QC). |
| Modification d'une résidence | Conforme | Le formulaire réutilise les valeurs existantes, y compris les commodités déjà associées (état `IsChecked`). Les descriptions de commodités sont pré-remplies et modifiables. |
| Détails d'une résidence | Conforme | La vue affiche l'adresse complète, le nombre d'unités (admin/gestionnaire uniquement), les commodités avec descriptions cliquables via modale, et le bouton "Faire une demande" adapté selon le rôle et la disponibilité. |
| Gestion des unités | Conforme | La liste des unités est paginée, filtrée par résidence, capacité, numéro et accessibilité. Le sélecteur de résidence permet de naviguer dynamiquement. |
| Ajout d'unités en lot | Conforme | Le formulaire permet de créer entre 1 et 50 unités à la fois avec numérotation automatique. La limite maximale est affichée directement dans l'interface. |
| Modification d'une unité | Conforme | Le formulaire affiche la résidence actuelle en en-tête et permet de modifier le numéro, la capacité, la résidence et l'accessibilité. |
| Gestion des commodités | Conforme | La liste est paginée et filtrée par nom. L'ajout, la modification et la suppression passent par des modales (`<dialog>`). |
| Pages Identity | Conforme | Les pages de connexion, d'inscription et de gestion de compte ont été intégrées et adaptées au projet. |
| Messages utilisateur | Conforme | Les actions utilisent `TempData` pour confirmer les succès ou afficher les erreurs sur toutes les opérations. |
| Suppression | Conforme | Toutes les suppressions passent par une modale de confirmation (`<dialog class="danger">`) avant l'envoi du formulaire. |
| Validation | Conforme | Les messages de validation sont en français et associés aux champs du formulaire (`ModelState.AddModelError`). |
| États vides | Conforme | Les vues affichent des messages contextuels distincts selon que la liste est vide à cause de filtres actifs ou qu'il n'y a simplement aucune donnée. |
| Accessibilité rôle gestionnaire | Conforme | Le bouton "Supprimer" est visible mais désactivé pour les gestionnaires, avec un tooltip explicatif indiquant qu'il faut être Admin. |
| Bouton "Faire une demande" | Conforme | Le bouton s'adapte selon l'état de connexion : non connecté → modale connexion, étudiant → lien vers `Demande/Creer`, autre rôle → modale profil étudiant requis, aucune unité → modale indisponible. |

---

## Revue de sécurité

| Élément vérifié | État | Notes |
| --- | --- | --- |
| Authentification obligatoire | Conforme | `ResidenceController` porte `[Authorize]` global ; `[AllowAnonymous]` est utilisé explicitement seulement pour `Index` et `ResidenceDetails`. |
| Politiques d'autorisation | Conforme | `AdminOuGestionnaire` protège l'ajout, la modification et la liste des unités/commodités. `AdminUniquement` protège toutes les suppressions. |
| Protection CSRF | Conforme | Tous les formulaires POST utilisent `[ValidateAntiForgeryToken]`. Les modales de suppression utilisent des `<form>` avec token AntiForgery intégré via Tag Helpers. Les tests d'intégration récupèrent et transmettent le token à chaque requête POST. |
| Validation côté serveur | Conforme | Les champs obligatoires, la vérification de doublons de noms, la complétude de l'adresse et la validité des commodités sont tous vérifiés côté serveur. |
| Vérification d'existence avant action | Conforme | Chaque action vérifie que la ressource existe avant de procéder (`GetById` + vérification `null` + `TempData["Erreur"]` + redirection). |
| Doublon de ressource | Conforme | `NomExiste` est appelé avant la création et la modification pour les résidences et les commodités. `UniteExiste` est appelé pour les unités. |
| Limite de création en lot | Conforme | La création d'unités en lot est limitée à 50 unités maximum avec validation côté serveur. |
| Commodités invalides | Conforme | `AjoutCommoditesChoisies` lève une exception si une commodité n'existe pas dans le repository, captée et traduite en erreur de modèle. |
| Erreurs serveur | Conforme | Les blocs `try/catch` retournent une vue `Erreur` avec code 500 pour toutes les opérations d'écriture. |
| Gestion de l'identité | Conforme | Les pages Identity nécessaires à l'authentification et à la gestion de compte ont été intégrées et sont protégées par les mécanismes d'ASP.NET Core Identity. |

---

## Couverture des tests

### Résidences

| Test | Type | Ce qui est vérifié |
| --- | --- | --- |
| `IndexRetourneVueResidences` | Unitaire | L'action `Index` retourne bien la vue nommée `"Residences"`. |
| `IndexMetBonTitre` | Unitaire | Le titre `ViewData["Title"]` est correctement défini à `"Résidences"`. |
| `CreerRedirigeIndex` | Unitaire | Une création valide redirige vers `Index`. |
| `CreerAppelleRepository` | Unitaire | Le repository est appelé exactement une fois lors d'une création valide. |
| `CreerNomExiste` | Unitaire | Le repository n'est pas appelé si le nom existe déjà. |
| `ModifierPostRedirige` | Unitaire | Une modification valide redirige vers `Index`. |
| `ModifierAppelleRepository` | Unitaire | Le repository est appelé lors d'une modification valide. |
| `SupprimerAppelleRepository` | Unitaire | Le repository est appelé lors d'une suppression. |
| `AjouterRefuseNonConnecte` | Intégration | Un utilisateur non connecté reçoit `401 Unauthorized` sur `AjouterResidence`. |
| `AjouterAccessibleAdmin` | Intégration | Un administrateur reçoit `200 OK` sur `AjouterResidence`. |
| `CreerValide` | Intégration | Une soumission valide ajoute bien une résidence dans le repository. |
| `CreerInvalide` | Intégration | Une soumission invalide (nom vide) ne modifie pas les données. |
| `CreationRefuseNonAdmin` | Intégration | Un utilisateur non connecté ne peut pas créer de résidence via POST. |
| `ModifierValide` | Intégration | Une modification valide met à jour le nom, l'adresse et le code postal dans le repository. |
| `ModificationRefuseNonAdmin` | Intégration | Un utilisateur non connecté ne peut pas modifier une résidence via POST. |
| `SupprimerFonctionne` | Intégration | La suppression d'une résidence existante diminue le compte du repository de 1. |
| `SuppressionRefuseNonAdmin` | Intégration | Un utilisateur non connecté ne peut pas supprimer une résidence via POST. |
| `RouteExiste` | Intégration (routage) | Les routes `/Residence` et `/Residence/ResidenceDetails/1` retournent `200 OK`. |
| `RouteNexistePas` | Intégration (routage) | Une route invalide retourne `404 Not Found`. |
| `VueAffichePropriete` | Intégration (routage) | La vue de détails affiche correctement le nom de la résidence dans le HTML. |

### Unités

| Test | Type | Ce qui est vérifié |
| --- | --- | --- |
| `CreerValide` | Intégration | Un admin peut créer une unité ; le repository augmente de 1. |
| `CreerPlusieursValide` | Intégration | La création en lot de 3 unités augmente le repository de 3. |
| `CreerInvalideNombreZero` | Intégration | Une création avec `nombreUnites = 0` est refusée côté serveur. |
| `CreerInvalideNombreTropGrand` | Intégration | Une création avec `nombreUnites = 51` est refusée côté serveur. |
| `CreerRefuseNonAdmin` | Intégration | Un utilisateur non connecté ne peut pas créer d'unité via POST. |
| `ModifierValide` | Intégration | Une modification valide met à jour le numéro et la capacité de l'unité dans le repository. |
| `ModifierInvalideNumeroExistant` | Intégration | La modification est refusée si le numéro est déjà utilisé dans la même résidence. |
| `ModifierRefuseNonAdmin` | Intégration | Un utilisateur non connecté ne peut pas modifier une unité via POST. |
| `SupprimerValide` | Intégration | La suppression d'une unité existante diminue le compte du repository de 1 et la rend introuvable. |
| `SupprimerInexistant` | Intégration | La suppression d'une unité inexistante ne fait pas planter l'application. |
| `SupprimerRefuseNonAdmin` | Intégration | Un utilisateur non connecté ne peut pas supprimer une unité via POST. |
| `IndexRetourneOk` | Intégration (routage) | La route `/Unite?id=1` retourne `200 OK` pour un admin. |
| `IndexRefuseNonConnecte` | Intégration (routage) | La route `/Unite` retourne une erreur si l'utilisateur n'est pas connecté. |
| `AjouterUniteRetourneOk` | Intégration (routage) | La page d'ajout d'unité retourne `200 OK` pour un admin. |
| `ModifierUniteRetourneOk` | Intégration (routage) | La page de modification d'une unité existante retourne `200 OK`. |
| `ModifierUniteInexistanteRetourneRedirection` | Intégration (routage) | La modification d'une unité inexistante redirige sans planter. |
| `VueAfficheNumeroUnite` | Intégration (routage) | La vue de modification affiche correctement le numéro de l'unité dans le HTML. |

---

## Corrections appliquées après audit

- Vérification `null` systématique sur toutes les ressources avant action (`GetById`, `GetCommodite`).
- Validation de la limite de 50 unités ajoutée côté serveur dans `UniteController.Creer`.
- Gestion explicite des états vides distincts (filtres actifs vs. aucune donnée) dans les trois vues principales.
- Comportement du bouton « Faire une demande » adapté à tous les cas : non connecté, étudiant, autre rôle, aucune unité disponible.
- Intégration et personnalisation des pages Identity nécessaires à l'authentification et à la gestion de compte.

---

## Conclusion

L'ensemble des fonctionnalités que j'ai développées (gestion des résidences, des unités et des commodités)  est couvert par 8 tests unitaires et 28 tests d'intégration. J'ai porté une attention particulière à la sécurité : les pages publiques sont accessibles sans connexion, les actions d'écriture exigent un rôle gestionnaire ou administrateur, et les suppressions sont réservées aux administrateurs uniquement. Les interfaces ont également été conçues pour refléter ces restrictions directement dans l'UI, notamment via les boutons désactivés avec tooltip pour les gestionnaires.