// Change main product image when thumbnail is clicked
function changeImage(element) {
    const mainImage = document.getElementById('mainImage');
    mainImage.src = element.src;

    // Update active thumbnail
    document.querySelectorAll('.thumbnail').forEach(thumb => {
        thumb.classList.remove('active');
    });
    element.classList.add('active');
}

// Quantity increment/decrement
document.getElementById('increment').addEventListener('click', function () {
    const quantityInput = document.getElementById('quantity');
    quantityInput.value = parseInt(quantityInput.value) + 1;
});

document.getElementById('decrement').addEventListener('click', function () {
    const quantityInput = document.getElementById('quantity');
    if (parseInt(quantityInput.value) > 1) {
        quantityInput.value = parseInt(quantityInput.value) - 1;
    }
});

// Star rating for review form
const stars = document.querySelectorAll('.rating i');
stars.forEach((star, index) => {
    star.addEventListener('click', () => {
        stars.forEach((s, i) => {
            if (i <= index) {
                s.classList.add('bi-star-fill');
                s.classList.remove('bi-star');
            } else {
                s.classList.add('bi-star');
                s.classList.remove('bi-star-fill');
            }
        });
    });
});