# Audit d'interface et revue de securite
## Portee

Cet audit couvre les pages et controles relies aux demandes de residence:

- `Demande/Index`
- `Demande/Creer`
- `Demande/Modifier`
- `Demande/Demandes`
- `GestionDemande/Index`
- exports PDF des demandes

## Audit de l'interface

| Element verifie | Etat | Notes |
| --- | --- | --- |
| Navigation principale | Conforme | Les pages de demandes sont accessibles selon le role de l'utilisateur connecte. |
| Page etat de la demande | Conforme | La page affiche la periode d'inscription ouverte/fermee et si la derniere demande est validee et soumise. |
| Creation d'une demande | Conforme | Le formulaire affiche les semestres, les genres, les consentements obligatoires, les informations du garant et les jumelages. |
| Copie d'une ancienne demande | Conforme | Le lien utilise maintenant l'id de la derniere demande de l'etudiant au lieu d'un id fixe. |
| Modification d'une demande | Conforme | Le formulaire reutilise les valeurs existantes et permet de modifier le semestre, les genres et les informations obligatoires. |
| Gestion des demandes | Conforme | Les gestionnaires et administrateurs peuvent filtrer par semestre, traiter les demandes et exporter les resultats. |
| Messages utilisateur | Conforme | Les actions importantes utilisent `TempData` pour confirmer les succes ou afficher les erreurs. |
| Suppression | Conforme | La suppression passe par une confirmation dans l'interface avant l'envoi du formulaire. |
| Validation | Conforme | Les messages de validation importants sont en francais et associes aux champs du formulaire. |
| Responsive | A verifier manuellement | Les vues utilisent Bootstrap. Une verification finale dans un navigateur mobile est recommandee avant la remise. |

## Revue de securite

| Element verifie | Etat | Notes |
| --- | --- | --- |
| Authentification obligatoire | Conforme | Les actions sensibles utilisent `[Authorize]` ou une politique d'autorisation. |
| Politiques d'autorisation | Conforme | `EstEtudiant` protege la creation et l'espace personnel; `AdminOuGestionnaire` protege la gestion. |
| Proprietaire de la demande | Conforme | Les actions de consultation, modification, export et suppression verifient que l'etudiant connecte est proprietaire lorsque necessaire. |
| Protection CSRF | Conforme | Les formulaires POST utilisent `[ValidateAntiForgeryToken]`. |
| Validation cote serveur | Conforme | Les champs obligatoires, consentements, semestre ouvert, genre et courriels de jumelage sont verifies cote serveur. |
| Periode d'inscription | Conforme | Une demande ne peut etre creee ou modifiee que si le semestre selectionne est ouvert. |
| Doublon de demande | Conforme | Un etudiant ne peut pas creer deux demandes pour le meme semestre. |
| Export PDF | Conforme | L'etudiant peut exporter sa demande; les administrateurs et gestionnaires peuvent exporter les demandes autorisees. |
| Donnees d'entree | Conforme | Les valeurs critiques sont relues cote serveur au lieu de faire confiance uniquement au formulaire. |
| Informations sensibles | A surveiller | Les fichiers `appsettings*.json` ne doivent pas contenir de secrets de production avant la remise. |

## Corrections appliquees apres audit

- Retrait du lien de copie avec id hardcode dans `Demande/Index`.
- Validation null-safe de `DateNaissanceGarant` pour eviter un crash si la date est absente.
- Ajout d'un test de modification valide pour verifier le changement reel d'une demande.
- Ajout de documentation XML sur les methodes principales de `DemandeController` et `GestionDemandeController`.

## Conclusion

Les fonctionnalites principales de demande et de gestion des demandes sont couvertes par les tests automatises et les controles d'autorisation. Les points a verifier avant la remise sont le deploiement Azure, les secrets dans les fichiers de configuration, et un dernier parcours manuel des pages dans un navigateur.
