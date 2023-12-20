//NOTE: If Error is Unthorized Redirect to LoginPage

function getURL() {
    return window.location.href;
}
function redirectToLogin() {
    window.location.href = '/Identity/Account/Login';
}
function addToCart(bookId, quantity) {

    delayInMilliseconds = 500 

    setTimeout(function () {
        addToCartNonDelay(bookId, quantity);
    }, delayInMilliseconds);
}

function addToCartNonDelay(bookId, quantity) {
    let url = `/api/carts/AddToCart/${bookId}/${quantity}`;
    // Use for Redirect to LoginPage
    $.ajax({
        url: url,
        type: 'POST',
        contentType: 'application/json',
        success: function (response) {
            alert("Add to cart sucess");
        },
        error: function (error) {
            redirectToLogin();
        }
    });
}

function editCart(bookId) {

    delayInMilliseconds = 300 // 2sec

    setTimeout(function () {
        editCartNonDelay(bookId);
    }, delayInMilliseconds);
}


function editCartNonDelay(bookId) {
    // Remove the incomplete setTimeout here

    var quantityElement = document.getElementById(`cart-item-${bookId}-quantity`);
    var quantity = quantityElement.value; // Get the value property

    let url = `/api/carts/edit/${bookId}/${quantity}`;
    $.ajax({
        url: url,
        type: 'PATCH',
        contentType: 'application/json',
        success: function (response) {
            console.log('Item Updated:', response);
        },
        error: function (error) {
            console.error('Error updating item in cart:', error);
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

//updateCartItem

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
    $("#checkout").html("Processing...")
    $('#checkout').prop('disabled', true);

    $.ajax({
        url: "/api/carts/checkout",
        type: "POST",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            $("#checkout").html("Done")
            // Disable the button
            $('#checkout').prop('disabled', true);
            if (isVnPaySelected()) {

                window.location.href = `/orders/pay/${data.orderId}`
            } else {
                window.location.href = "/orders/myorders"
            }
        },
        error: function (xhr, textStatus, errorThrown) {
            htmlAlert = '<div id="alert-message" class="alert alert-danger alert-dismissible" role="alert">' +
                '<button type = "button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>' +
                xhr.responseJSON.message +
                '</div >';

            $("#alert-message").html(htmlAlert);
            $('#checkout').prop('disabled', false);
            $("#checkout").html("place order")

            setTimeout(function () {
                $("#alert-message").html("");
            }, 3000);
        }
    });
}