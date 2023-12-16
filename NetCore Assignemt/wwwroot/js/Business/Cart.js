﻿//NOTE: If Error is Unthorized Redirect to LoginPage

function getURL() {
    return window.location.href;
}
function redirectToLogin() {
    window.location.href = '/Identity/Account/Login';
}

function addToCart(bookId, quantity) {

    let url = `/api/carts/AddToCart/${bookId}/${quantity}`;
    // Use for Redirect to LoginPage
    let returnURL = getURL();
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        success: function (response) {
            console.log('Item added to cart:', response);
            // Change classes
            var button = $(this);

            button.removeClass('btn-primary').addClass('btn-success');
            button.find('.addToCartIcon').removeClass('fa-cart-shopping').addClass('fa-check');

            // Set a timeout to revert the changes after 1 second
            setTimeout(function () {
                button.removeClass('btn-success').addClass('btn-primary');
                button.find('.addToCartIcon').removeClass('fa-check').addClass('fa-cart-shopping');
            }, 1000);
        },
        error: function (error) {
            redirectToLogin();
        }
    });
}
function editCart(bookId, quatity) {

    delayInMilliseconds = 2000 // 2sec

    setTimeout(function () {
        editCartNonDelay(bookId, quatity);
    }, delayInMilliseconds);
}

function editCartNonDelay(bookId) {
    setTimeout()
    var quantity = document.getElementById(`cart-item-${bookId}-quantity`);

    let url = `/api/carts/edit/${bookId}/${quantity}`;

    $.ajax({
        url: url,
        type: 'PATCH',
        contentType: 'application/json',
        success: function (response) {
            console.log('Item Updated:', response);

        },
        error: function (error) {
            console.error('Error adding item to cart:', error);
        }
    });
}
function getCart() {
    $.ajax({
        url: '/api/carts/',
        type: 'GET',
        success: function (data) {
            console.log(data);
            // Handle the successful response, 'data' will contain the cart information
        },
        error: function (error) {
            console.error(error);
        }
    });

}
function getCart(size) {
    $.ajax({
        url: `/api/carts/?size=${size}`,
        type: 'GET',
        success: function (data) {
            console.log(data);
            // Handle the successful response, 'data' will contain the cart information
        },
        error: function (error) {
            console.error(error);
        }
    });

}
// Delete a specific book from the cart
function deleteCartItem(bookId) {
    $.ajax({
        url: `/api/Carts/delete/${bookId}`,
        type: 'DELETE',
        success: function (data) {
            $(`#cart-item-${bookId}`).remove();
        },
        error: function (error) {
            // Handle errors, e.g., display an error message
            console.error(error.responseJSON.Message);
        }
    });
}

// Delete all items from the cart
function deleteAllCartItems() {
    alert("Delete All Book in the Cart. This action can not be undone!");
    $.ajax({
        url: '/api/Carts/deleteall',
        type: 'DELETE',
        success: function (data) {
            // Handle success, e.g., show a message or update the UI
            console.log(data.Message);
        },
        error: function (error) {
            // Handle errors, e.g., display an error message
            console.error(error.responseJSON.Message);
        }
    });


}


function isVnPaySelected() {
    return $('#VnPay-1').is(':checked');
}


function checkOut() {
    alert("Are you Sure?. This action can not be undone!");
    $.ajax({
        url: "/api/carts/checkout",  // Replace with the actual URL of your controller
        type: "POST",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        headers: {
            // Add any additional headers if needed, such as authorization headers
        },
        success: function (data) {
            console.log(data);

            $("#checkout").html("Done")
            if (isVnPaySelected()) {

            } else {
                window.location.href = "/orders/return"
            }

        },
        error: function (xhr, textStatus, errorThrown) {
            // Handle the error response
            $("#checkout").html(xhr.Message)

                setTimeout(function () {
                    $('#checkout').html('Place Order');
                }, 2000); // C
        }
    });
}