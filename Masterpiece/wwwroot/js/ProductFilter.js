//    document.addEventListener('DOMContentLoaded', function () {
//    const applyBtn = document.querySelector('.btn-filter');
//    const resetBtn = document.querySelector('.btn-reset');

//    applyBtn.addEventListener('click', function () {
//        const selectedFilters = {
//        categories: [],
//    priceMax: parseFloat(document.getElementById('priceRange').value),
//    ratings: [],
//    color: null,
//    size: null,
//    dateOrder: null,
//        };

//        // Get selected categories
//        document.querySelectorAll('#categoryFilter input[type="checkbox"]:checked').forEach(cb => {
//        selectedFilters.categories.push(cb.nextElementSibling.innerText.trim());
//        });

//        // Get selected ratings
//        document.querySelectorAll('#ratingFilter input[type="checkbox"]:checked').forEach(cb => {
//        selectedFilters.ratings.push(parseInt(cb.id.replace('rating', '')));
//        });

//    // Get selected color
//    const activeColor = document.querySelector('.color-option.active');
//    if (activeColor) {
//        selectedFilters.color = activeColor.getAttribute('title');
//        }

//    // Get selected size
//    const selectedSize = document.querySelector('#sizeFilter input[type="radio"]:checked');
//    if (selectedSize) {
//        selectedFilters.size = selectedSize.nextElementSibling.innerText;
//        }

//    // Get selected date order
//    const selectedDate = document.querySelector('#dateFilter input[type="radio"]:checked');
//    if (selectedDate) {
//        selectedFilters.dateOrder = selectedDate.nextElementSibling.innerText.trim();
//        }

//    // Apply the filter to products
//    filterProducts(selectedFilters);
//    });

//    resetBtn.addEventListener('click', function () {
//        // Reset checkboxes and radio buttons
//        document.querySelectorAll('.form-check-input').forEach(input => input.checked = false);
//        document.querySelectorAll('.btn-check').forEach(input => input.checked = false);

//        // Reset color selection
//        document.querySelectorAll('.color-option').forEach(opt => opt.classList.remove('active'));

//    // Reset price range
//    document.getElementById('priceRange').value = 1000; // Reset to max
//    document.getElementById('maxPrice').textContent = "1000";

//        // Show all products
//        document.querySelectorAll('.product-card').forEach(product => {
//        product.parentElement.style.display = 'block';
//        });
//    });

//    function filterProducts(filters) {
//        document.querySelectorAll('.product-card').forEach(product => {
//            const productCategory = product.querySelector('h5').innerText.trim(); // Get product title as category example
//            const productPrice = parseFloat(product.querySelector('.product-price').innerText.replace('$', ''));
//            const productRating = product.querySelectorAll('.bi-star-fill').length; // Count filled stars
//            const productColors = [...product.querySelectorAll('.color-option')].map(color => color.getAttribute('title'));

//            let showProduct = true;

//            // Category Filter
//            if (filters.categories.length > 0 && !filters.categories.includes(productCategory)) {
//                showProduct = false;
//            }

//            // Price Filter
//            if (productPrice > filters.priceMax) {
//                showProduct = false;
//            }

//            // Rating Filter
//            if (filters.ratings.length > 0 && !filters.ratings.some(rating => productRating >= rating)) {
//                showProduct = false;
//            }

//            // Color Filter
//            if (filters.color && !productColors.includes(filters.color)) {
//                showProduct = false;
//            }

//            // Show or hide product
//            product.parentElement.style.display = showProduct ? 'block' : 'none';
//        });
//    }
//});

    function filterProducts() {
    const filters = {
        categories: [...document.querySelectorAll('input[name="category"]:checked')].map(cb => cb.value),
        ratings: [...document.querySelectorAll('input[name="rating"]:checked')].map(cb => parseInt(cb.value)),
    color: document.querySelector('input[name="color"]:checked')?.value || null,
    size: document.querySelector('input[name="size"]:checked')?.value || null,
    dateOrder: document.querySelector('input[name="date"]:checked')?.value || null,
    priceMax: parseInt(document.getElementById("priceRange").value) || 1000
    };

    document.querySelectorAll('.product-card').forEach(product => {
        const productCategory = product.getAttribute('data-category');
    const productRating = parseFloat(product.getAttribute('data-rating'));
    const productColors = product.getAttribute('data-colors')?.split(',') || [];
    const productPrice = parseFloat(product.querySelector('.price')?.innerText.replace("€", "").trim());

    let show = true;

        if (filters.categories.length > 0 && !filters.categories.includes(productCategory)) {
        show = false;
        }

        if (filters.ratings.length > 0 && !filters.ratings.includes(Math.floor(productRating))) {
        show = false;
        }

    if (filters.color && !productColors.includes(filters.color)) {
        show = false;
        }

        if (productPrice > filters.priceMax) {
        show = false;
        }

    product.parentElement.style.display = show ? 'block' : 'none';
    });
}

    // Call this when the page loads to show everything by default
    document.addEventListener("DOMContentLoaded", filterProducts);

// Attach event listeners
document.querySelectorAll('input[name="category"], input[name="rating"], input[name="color"], input[name="size"], input[name="date"], #priceRange').forEach(input => {
        input.addEventListener("change", filterProducts);
});

