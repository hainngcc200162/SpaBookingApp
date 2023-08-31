
document.addEventListener("DOMContentLoaded", function () {
    const orderForm = document.getElementById("orderForm");
    const responseContainer = document.getElementById("responseContainer");

    orderForm.addEventListener("submit", async function (event) {
        event.preventDefault();

        var token = sessionStorage.getItem("Token");

        if (!token) {
            window.location.href = "/error/AccessDenied.html";
            return;
        }

        var deliveryAddress = document.getElementById("deliveryAddress").value;
        var phoneNumber = document.getElementById("phoneNumber").value;
        var paymentMethod = document.getElementById("paymentMethod").value;
        var paymentStatus = "Accepted";

        if (paymentMethod === "Cash") {
            const orderData = {
                deliveryAddress: deliveryAddress,
                phoneNumber: phoneNumber,
                paymentMethod: paymentMethod,
                paymentStatus: paymentStatus
            };

            axios.post("/api/Order", orderData, {
                headers: {
                    Authorization: "Bearer " + token,
                    "Content-Type": "application/json",
                },
            })
                .then(response => {
                    if (response.data.success) {
                        alert("Order created successfully");
                        window.location.href = "/Home/Index"; // Điều hướng đến trang danh sách đơn hàng
                    } else {
                        console.log("Error: " + response.data.message);
                    }
                })
                .catch(error => {
                    window.location.href = "/Error/AccessDenied.html";
                });
        }
    });
});



function generateRandomString(length) {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';
    for (let i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * characters.length));
    }
    return result;
}

document.addEventListener("DOMContentLoaded", function () {
    const orderForm = document.getElementById("orderForm");
    const responseContainer = document.getElementById("responseContainer");

    orderForm.addEventListener("submit", async function (event) {
        event.preventDefault();

        var token = sessionStorage.getItem("Token");

        if (!token) {
            window.location.href = "/error/AccessDenied.html";
            return;
        }

        var deliveryAddress = document.getElementById("deliveryAddress").value;
        var phoneNumber = document.getElementById("phoneNumber").value;
        var paymentMethod = document.getElementById("paymentMethod").value;

        sessionStorage.setItem("deliveryAddress", deliveryAddress);
        sessionStorage.setItem("phoneNumber", phoneNumber);
        sessionStorage.setItem("paymentMethod", paymentMethod);

        if (paymentMethod === "Card") {
            const randomString = generateRandomString(10); // Độ dài chuỗi ngẫu nhiên là 10

            sessionStorage.setItem("randomString", randomString);

            console.log(randomString);

            const stripeResponse = await fetch("/api/Stripe/checkout", {
                method: "POST",
                headers: {
                    Accept: "application/json",
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(randomString) // Chỉ truyền chuỗi ngẫu nhiên vào body
            });

            const stripeData = await stripeResponse.json();
            responseContainer.innerHTML = JSON.stringify(stripeData, null, 2);

            window.location.href = stripeData.sessionUrl;
            // Xử lý stripeData tại đây, thông báo thanh toán thành công và cập nhật PaymentStatus, OrderStatus
        }
    });
});


async function removeCartItem(productId) {
    console.log(`Removing item with productId ${productId}`);
    const quantityInput = document.querySelector(`.quantity-input[data-product-id="${productId}"]`);
    quantityInput.value = 0;

    updateCart(productId);
    fetchCart();
}

async function updateCart(productId) {
    const quantityInput = document.querySelector(`.quantity-input[data-product-id="${productId}"]`);
    const quantity = parseInt(quantityInput.value);

    const cartItems = Array.from(document.querySelectorAll('.quantity-input'));
    const updatedProductIdentifiers = cartItems
        .filter(input => parseInt(input.value) > 0)
        .map(input => {
            const id = input.getAttribute('data-product-id');
            const q = (id === productId) ? quantity : parseInt(input.value);
            return Array(q).fill(id);
        })
        .flat()
        .join('-');

    try {
        const apiUrl = `/api/Cart/?productIdentifiers=${updatedProductIdentifiers}`;
        const response = await fetch(apiUrl, {
            method: 'PUT'
        });

        if (response.ok) {
            const cartData = await response.json();
            if (cartData.cartItems.length === 0) {
                cartData.cartItems = [];
            }
            fetchCart();
        } else {
            fetchCart();
        }
    } catch (error) {
        console.error('An error occurred while updating cart data:', error);
    }
}

async function fetchCart() {
    try {
        const response = await fetch('/api/Cart');
        if (response.ok) {
            const cartData = await response.json();
            updateCartView(cartData);
        } else {
            console.error('Failed to fetch cart data.');
        }
    } catch (error) {
        console.error('An error occurred while fetching cart data:', error);
    }
}

function updateCartView(cartData) {
    const cartItemsElement = document.getElementById('cart-items');
    const table = cartItemsElement.querySelector('table');

    if (cartData && cartData.cartItems && cartData.cartItems.length > 0) {
        const tbody = table.querySelector('tbody');
        tbody.innerHTML = '';
        table.className = "table-shopping-cart";

        const tableHeader = `
                <tr class="table_head text-center" >
                    <th class="column-1">Product</th>
                    <th class="column-2">Name</th>
                    <th class="column-3">Quantity</th>
                    <th class="column-4">Price</th>
                    <th class="column-5">Total</th>
                    <th></th>
                    <th class="column-6"></th>
                    <th></th>
                </tr>
            `;

        tbody.insertAdjacentHTML('beforeend', tableHeader);

        cartData.cartItems.forEach(item => {
            const row = tbody.insertRow();
            row.className = 'cart-item table_row';
            row.innerHTML = `
                    <td class="column-1">
                        <div class="how-itemcart1" >
                            <img src="${item.spaProduct.posterName}" alt="IMG">
                        </div>
                    </td>
                    <td class="column-2">${item.spaProduct.name}</td>
                    <td class="column-3 align="center">
                        <div class="wrap-num-product flex-w">
                            <div class="btn-num-product-down cl8 hov-btn3 trans-04 flex-c-m">
                                <i class="fs-16 zmdi zmdi-minus"></i>
                            </div>
                            <input type="number" class="quantity-input mtext-104 cl3 txt-center num-product" data-product-id="${item.spaProduct.id}" min="0" value="${item.quantity}">
                            <div class="btn-num-product-up cl8 hov-btn3 trans-04 flex-c-m">
                                <i class="fs-16 zmdi zmdi-plus"></i>
                            </div>
                        </div>
                    </td>

                    <td class="column-4">$${item.spaProduct.price}</td>

                    <td class="column-5">$${(item.spaProduct.price * item.quantity).toFixed(2)}</td>
                    
                    <td>
                        <button class="flex-c-m stext-101 cl2 size-105 bg8 bor13 hov-btn3 p-lr-15 trans-04 pointer m-tb-10" data-product-id="${item.spaProduct.id}" onclick="updateCart(${item.spaProduct.id})">Update</button>
                    </td>
                    <td class="column-6"></td>
                    <td>
                        <button class="flex-c-m stext-101 cl2 size-105 bg8 bor13 hov-btn3 p-lr-15 trans-04 pointer m-tb-10" data-product-id="${item.spaProduct.id}" onclick="removeCartItem(${item.spaProduct.id})">Remove</button>
                    </td>
                `;

            const plusButton = row.querySelector('.btn-num-product-up');
            const minusButton = row.querySelector('.btn-num-product-down');
            const quantityInput = row.querySelector('.quantity-input');

            let tempQuantity = parseInt(quantityInput.value);

            plusButton.addEventListener('click', () => {
                tempQuantity += 1;
                quantityInput.value = tempQuantity;
            });

            minusButton.addEventListener('click', () => {
                if (tempQuantity > 0) {
                    tempQuantity -= 1;
                    quantityInput.value = tempQuantity;
                }
            });
        });
    } else {
        table.innerHTML = '';
        cartItemsElement.innerHTML = '<p>Your cart is empty.</p>';
    }

    const cartSummaryElement = document.getElementById('cart-summary');
    cartSummaryElement.innerHTML = `
            <div class="cart-summary-container">
                <div class="cart-summary-item">
                    <span class="cart-summary-label"><strong>Shipping Fee:</strong></span>
                    <span class="cart-summary-value">$${cartData.shippingFee.toFixed(2)}</span>
                </div>
                
                <div class="cart-summary-item">
                    <span class="cart-summary-label"><strong>Total Price:</strong></span>
                    <span class="cart-summary-value">$${cartData.totalPrice.toFixed(2)}</span>
                </div>
            </div>
        `;
}

async function clearCart() {
    try {
        const response = await fetch('/api/Cart', {
            method: 'DELETE'
        });

        if (response.ok) {
            await fetchCart();
        } else {
            console.error('Failed to clear cart.');
        }
    } catch (error) {
        console.error('An error occurred while clearing cart:', error);
    }
}

fetchCart();
