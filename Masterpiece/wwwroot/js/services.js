///////*********cards****/ */
document.addEventListener("DOMContentLoaded", function () {
    const toggleButtons = document.querySelectorAll(".action-btn");

    toggleButtons.forEach((button) => {
        button.addEventListener("click", function () {
            const card = this.closest(".card");
            const content = card.querySelector(".card-content");
            content.classList.toggle("show");

            const content2 = card.querySelector(".card-container card");
            content2.classList.toggle("showcard");
        });
    });
});
