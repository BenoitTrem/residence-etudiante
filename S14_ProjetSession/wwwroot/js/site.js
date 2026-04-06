(() => {
    document.addEventListener("click", (evenement) => {
        const boutonOuvrir = evenement.target.closest("[data-dialog-open]");
        if (!boutonOuvrir) return;

        const idDialog = boutonOuvrir.dataset.dialogOpen;
        const dialog = document.getElementById(idDialog);

        dialog?.showModal();
        dialog.focus();
    });
    document.addEventListener("click", (evenement) => {
        const boutonFermer = evenement.target.closest("[data-dialog-close]");
        if (!boutonFermer) return;

        const dialog = boutonFermer.closest("dialog");
        dialog?.close();
    });
})();

document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll('.form-check-input').forEach(cb => {
        const input = cb.closest('li').querySelector('.description-input');
        if (!input) return;

        input.style.display = cb.checked ? 'block' : 'none';

        cb.addEventListener('change', () => {
            input.style.display = cb.checked ? 'block' : 'none';
        });
    });
});