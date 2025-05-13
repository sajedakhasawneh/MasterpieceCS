    document.addEventListener('DOMContentLoaded', function () {
        document.querySelectorAll('.quantity-form').forEach(form => {
            const input = form.querySelector('.quantity-input');
            const minusBtn = form.querySelector('.minus-btn');
            const plusBtn = form.querySelector('.plus-btn');

            function updateQuantity(newQuantity) {
                const cartItemId = form.dataset.cartItemId;

                fetch('/Cart/UpdateQuantity', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value || ''
                    },
                    body: JSON.stringify({
                        cartItemId: parseInt(cartItemId),
                        newQuantity: parseInt(newQuantity)
                    })
                })
                    .then(res => res.json())
                    .then(data => {
                        if (data.success) {
                            // Update totals
                            document.querySelector('#cart-subtotal').textContent = `$${data.subtotal.toFixed(2)}`;
                            document.querySelector('#cart-total').textContent = `$${data.total.toFixed(2)}`;
                            document.querySelector('.badge.bg-primary').textContent = `${data.itemCount} items`;
                        }
                    });
            }

            minusBtn.addEventListener('click', () => {
                let val = parseInt(input.value);
                if (val > 1) {
                    val -= 1;
                    input.value = val;
                    updateQuantity(val);
                }
            });

            plusBtn.addEventListener('click', () => {
                let val = parseInt(input.value);
                if (val < 99) {
                    val += 1;
                    input.value = val;
                    updateQuantity(val);
                }
            });

            input.addEventListener('change', () => {
                let val = parseInt(input.value);
                if (val < 1) val = 1;
                if (val > 99) val = 99;
                input.value = val;
                updateQuantity(val);
            });
        });
});

