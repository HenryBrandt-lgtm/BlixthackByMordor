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
            let count = document.querySelector("#favorite-count").innerHTML;
            count--;
            document.querySelector("#favorite-count").innerHTML = count;


        }

    });

});


function sortThreads(select) {
    const sortType = select.value;
    const categorytCard = select.closest(".category-card")
    const threadList = categorytCard.querySelector(".thread-list")
    const threads = Array.from(threadList.querySelectorAll(".thread-list__item"))
    
    threads.sort((threadA, threadB) => {
        if (sortType === "latest") {
            return Number(threadB.dataset.created) - Number(threadA.dataset.created)
        }

        if (sortType === "oldest") {
            return Number(threadA.dataset.created) - Number(threadB.dataset.created);
        }

        if (sortType === "most-answers") {
            return Number(threadB.dataset.answers) - Number(threadA.dataset.answers);
        }

    })

    threads.forEach(thread => {
        threadList.appendChild(thread)
    })
}
