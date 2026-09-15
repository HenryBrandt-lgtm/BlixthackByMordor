document.querySelectorAll(".favorite-checkbox").forEach(checkbox => {
    checkbox.addEventListener("change", async function () {
        const answerId = this.dataset.answerId;
        const response = await fetch(`/UserFavorite/ToggleFavorite?answerId=${answerId}`, {
            method: "POST"
        });

        if (!response.ok) {
            this.checked = !this.checked;
            return;
        }

        const result = await response.json();
        this.checked = result.isFavorite;

        if (!result.isFavorite) {
            this.closest("[data-thread-item]")?.remove();
            const countEl = document.querySelector("#favorite-count");
            if (countEl) {
                countEl.textContent = String(Number(countEl.textContent) - 1);
            }
        }
    });
});

function sortThreads(card, sortType) {
    const threadList = card.querySelector("[data-thread-list]");
    const threads = Array.from(threadList.querySelectorAll("[data-thread-item]"));

    threads.sort((threadA, threadB) => {
        if (sortType === "latest") {
            return Number(threadB.dataset.created) - Number(threadA.dataset.created);
        }

        if (sortType === "oldest") {
            return Number(threadA.dataset.created) - Number(threadB.dataset.created);
        }

        if (sortType === "most-answers") {
            return Number(threadB.dataset.answers) - Number(threadA.dataset.answers);
        }

        return 0;
    });

    threads.forEach(thread => {
        threadList.appendChild(thread);
    });
}

(function () {
    const navToggle = document.getElementById("nav-toggle");
    const navMenu = document.getElementById("site-nav-menu");

    if (navToggle && navMenu) {
        navToggle.addEventListener("click", () => {
            const isOpen = navMenu.classList.toggle("is-open");
            navToggle.setAttribute("aria-expanded", String(isOpen));
        });
    }

    const closeDropdowns = () => {
        document.querySelectorAll("[data-dropdown]").forEach((dropdown) => {
            const trigger = dropdown.querySelector("[data-dropdown-trigger]");
            const menu = dropdown.querySelector("[data-dropdown-menu]");
            if (menu) {
                menu.hidden = true;
            }
            if (trigger) {
                trigger.setAttribute("aria-expanded", "false");
            }
        });
    };

    document.querySelectorAll("[data-dropdown]").forEach((dropdown) => {
        const trigger = dropdown.querySelector("[data-dropdown-trigger]");
        const menu = dropdown.querySelector("[data-dropdown-menu]");
        if (!trigger || !menu) {
            return;
        }

        trigger.addEventListener("click", (event) => {
            event.stopPropagation();
            const willOpen = menu.hidden;
            closeDropdowns();
            if (willOpen) {
                menu.hidden = false;
                trigger.setAttribute("aria-expanded", "true");
            }
        });

        menu.addEventListener("click", (event) => {
            event.stopPropagation();
        });
    });

    document.querySelectorAll("[data-sort-dropdown]").forEach((dropdown) => {
        const label = dropdown.querySelector("[data-sort-label]");
        const card = dropdown.closest("[data-category-card]");

        dropdown.querySelectorAll("[data-sort]").forEach((option) => {
            option.addEventListener("click", () => {
                const sortType = option.dataset.sort;
                if (label) {
                    label.textContent = option.textContent.trim();
                }

                dropdown.querySelectorAll("[data-sort]").forEach((item) => {
                    item.classList.toggle("is-current", item === option);
                });

                if (card) {
                    sortThreads(card, sortType);
                }
                closeDropdowns();
            });
        });
    });

    document.querySelectorAll("[data-reply-toggle]").forEach((button) => {
        button.addEventListener("click", () => {
            const form = document.getElementById(`create-reply-form-${button.dataset.replyToggle}`);
            if (!form) {
                return;
            }

            const willShow = form.hidden;
            document.querySelectorAll(".comment-composer").forEach((other) => {
                other.hidden = true;
            });
            form.hidden = !willShow;
            if (!form.hidden) {
                form.querySelector("textarea")?.focus();
            }
        });
    });

    document.querySelectorAll("[data-reply-cancel]").forEach((button) => {
        button.addEventListener("click", () => {
            const form = document.getElementById(`create-reply-form-${button.dataset.replyCancel}`);
            if (form) {
                form.hidden = true;
            }
        });
    });

    document.addEventListener("click", closeDropdowns);
    document.addEventListener("keydown", (event) => {
        if (event.key === "Escape") {
            closeDropdowns();
        }
    });
})();
