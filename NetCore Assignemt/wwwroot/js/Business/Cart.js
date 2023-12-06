//NOTE: If Error is Unthorized Redirect to LoginPage

function getURL() {
    return window.location.href;
}
function redirectToLogin(returnUrl) {

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
        },
        error: function (error) {
            console.error('Error adding item to cart:', error);
            // If Unthorized Redirect to LoginPage

            window.location.href = newPage;
        }
    });
}
function editCart(bookId, quatity) {

    delayInMilliseconds = 2000 // 2sec

    setTimeout(function () {
        editCartNonDelay(bookId, quatity);
    }, delayInMilliseconds);
}

function editCartNonDelay(bookId, quantity) {
    setTimeout()
    let url = `/api/carts/edit/${bookId}/${quantity}`;

    $.ajax({
        url: url,
        type: 'PATCH',
        contentType: 'application/json',
        success: function (response) {
            console.log('Item Updated:', response);

            // Handle success, maybe update UI, show a message, etc.
        },
        error: function (error) {
            console.error('Error adding item to cart:', error);

            // Handle error, show an error message, etc.
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
            // Handle success, e.g., show a message or update the UI
            console.log(data.Message);
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
            // Handle the success response
            console.log("Checkout successful:", data);
            // Redirect to a success page or update UI accordingly
        },
        error: function (xhr, textStatus, errorThrown) {
            // Handle the error response
            console.error("Checkout failed:", xhr.responseText);
            // Display an error message or redirect to an error page
        }
    });
}