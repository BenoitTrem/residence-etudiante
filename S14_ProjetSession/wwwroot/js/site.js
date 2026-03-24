(() => {
    document.addEventListener("click", (evenement) => {
        const boutonOuvrir = evenement.target.closest("[data-dialog-open]");
        if (!boutonOuvrir) return;

        const idDialog = boutonOuvrir.dataset.dialogOpen;
        const dialog = document.getElementById(idDialog);

        dialog?.showModal();
    });
    document.addEventListener("click", (evenement) => {
        const boutonFermer = evenement.target.closest("[data-dialog-close]");
        if (!boutonFermer) return;

        const dialog = boutonFermer.closest("dialog");
        dialog?.close();
    });
})();