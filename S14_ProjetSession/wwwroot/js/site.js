/*
 * @author Benoit
 * 
 * Description :
 * Script JavaScript permettant :
 * 1. D’ouvrir et de fermer des boîtes de dialogue (<dialog>) via des boutons.
 * 2. D’afficher ou cacher un champ textarea (description)
 *    lorsqu’une case à cocher (checkbox) est sélectionnée.
 * 
 * Note :
 * Une partie de ce code a été générée avec l’aide de ChatGPT. 
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