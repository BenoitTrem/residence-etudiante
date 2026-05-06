/*
 * @author Benoit
 * 
 * Description :
 * Script JavaScript permettant :
 * 1. D'ouvrir et de fermer des boîtes de dialogue (<dialog>) via des boutons.
 * 2. D'afficher ou cacher un champ textarea (description)
 *    lorsqu'une case à cocher (checkbox) est sélectionnée.
 * 3. De pré-sélectionner une résidence dans un menu déroulant au chargement de la page.
 * 4. De rediriger vers la liste des unités d'une résidence sélectionnée.
 * 5. De gérer un carrousel d'arrière-plan automatique avec navigation manuelle.
 * 6. D'ouvrir les dialogs de modification et de suppression de commodités.
 * 
 * Note :
 * Une partie de ce code a été générée avec l'aide de ChatGPT. 
 */

(() => {
    // Écoute les clics sur toute la page
    document.addEventListener("click", (evenement) => {
        // Vérifie si l'utilisateur a cliqué sur un bouton d'ouverture de dialog
        const boutonOuvrir = evenement.target.closest("[data-dialog-open]");
        if (!boutonOuvrir) return;

        // Récupère l'ID du dialog à ouvrir
        const idDialog = boutonOuvrir.dataset.dialogOpen;
        const dialog = document.getElementById(idDialog);

        // Ouvre le dialog et lui donne le focus
        dialog?.showModal();
        dialog.focus();
    });

    // Gestion de la fermeture des dialogs
    document.addEventListener("click", (evenement) => {
        // Vérifie si l'utilisateur a cliqué sur un bouton de fermeture
        const boutonFermer = evenement.target.closest("[data-dialog-close]");
        if (!boutonFermer) return;

        // Trouve le dialog parent et le ferme
        const dialog = boutonFermer.closest("dialog");
        dialog?.close();
    });
})();

// S'exécute lorsque le DOM est complètement chargé
document.addEventListener("DOMContentLoaded", () => {
    // Sélectionne toutes les cases à cocher
    document.querySelectorAll('.form-check-input').forEach(cb => {

        // Trouve le champ de description associé dans le même <li>
        const input = cb.closest('li').querySelector('.description-input');
        if (!input) return;

        // Affiche ou cache le textarea selon l’état initial de la checkbox
        input.style.display = cb.checked ? 'block' : 'none';

        // Ajoute un écouteur pour réagir aux changements de la checkbox
        cb.addEventListener('change', () => {
            input.style.display = cb.checked ? 'block' : 'none';
        });
    });
});

// Ouvre le dialog de modification et pré-remplit les champs avec les données de la commodité
function ouvreModiferModal(id, nom) {
    // Injecte l'identifiant et le nom de la commodité dans les champs du formulaire
    document.getElementById("idCommoditeModifier").value = id;
    document.getElementById("nomCommoditeModifier").value = nom;

    // Affiche le dialog de modification
    document.getElementById("modalModifier").showModal();    
}

// Ouvre le dialog de suppression et affiche le nom de la commodité à supprimer
function ouvreSupprimerModal(id, nom) {
    // Injecte l'identifiant dans le champ caché et affiche le nom dans le dialog
    document.getElementById("idCommoditeSupprimer").value = id;
    document.getElementById("nomCommoditeSupprimer").innerText = nom;

    // Affiche le dialog de suppression
    document.getElementById("modalSupprimer").showModal();
}

// S'exécute lorsque le DOM est complètement chargé
document.addEventListener('DOMContentLoaded', function () {

    // Récupère le menu déroulant de sélection de résidence
    const select = document.getElementById('residenceSelect');

    // Lit l'identifiant de la résidence pré-sélectionnée depuis l'attribut data
    const selectedResidenceId = select.getAttribute('data-selected');

    // Si aucune résidence pré-sélectionnée, ne fait rien
    if (!selectedResidenceId) return;

    // Parcourt les options du select pour trouver et sélectionner la bonne résidence
    for (let i = 0; i < select.options.length; i++) {
        if (select.options[i].value == selectedResidenceId) {
            select.selectedIndex = i;
            break;
        }
    }
});

// Redirige vers la liste des unités de la résidence sélectionnée
function loadUnits(residenceId) {
    window.location.href = '/Unite/Index?id=' + residenceId;
}

// Carrousel d'arrière-plan automatique
document.addEventListener('DOMContentLoaded', function () {

    // Sélectionne toutes les diapos et les indicateurs de navigation
    const diapos = document.querySelectorAll('.diapo');
    const indicateurs = document.querySelectorAll('.indicateur');

    if (!diapos.length) return;  // Si aucune diapo trouvée, on arrête

    let indexActuel = 0;
    const duree = 5000; // Durée entre chaque transition

    // Passe à la diapo correspondant à l'index donné
    function allerA(index) {
        // Retire la classe active de la diapo et de l'indicateur courants
        diapos[indexActuel].classList.remove('diapo-active');
        indicateurs[indexActuel].classList.remove('indicateur-actif');

        // Calcule le nouvel index en bouclant sur le nombre total de diapos
        indexActuel = (index + diapos.length) % diapos.length;

        // Active la nouvelle diapo et son indicateur
        diapos[indexActuel].classList.add('diapo-active');
        indicateurs[indexActuel].classList.add('indicateur-actif');
    }

    // Lance le défilement automatique toutes les millisecondes
    let minuterie = setInterval(() => allerA(indexActuel + 1), duree);

    // Permet la navigation manuelle via les indicateurs
    indicateurs.forEach((ind, i) => {
        ind.addEventListener('click', () => {
            // Arrête le défilement automatique avant de changer manuellement
            clearInterval(minuterie);
            allerA(i);

            // Relance le défilement automatique après la navigation manuelle
            minuterie = setInterval(() => allerA(indexActuel + 1), duree);
        });
    });
});