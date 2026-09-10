// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.querySelectorAll(".favorite-checkbox").forEach(checkbox => {

    checkbox.addEventListener("change", async function () {

        const answerId = this.dataset.answerId;
        console.log("Answer ID:", answerId);

        const response = await fetch(`/UserFavorite/ToggleFavorite?answerId=${answerId}`, {
            method: "POST"

        });
        console.log("Status:", response.status);

        if (!response.ok) {
            this.checked = !this.checked;
            return;
        }

        const result = await response.json();
        console.log("Server response:", result);

        this.checked = result.isFavorite;
        if (!result.isFavorite) {
            this.closest(".thread-list__item").remove();
        }

    });

});