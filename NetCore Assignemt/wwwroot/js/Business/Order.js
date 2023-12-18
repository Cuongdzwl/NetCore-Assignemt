function cancelOrder(orderId) {
    $.ajax({
        type: 'POST',
        url: '/api/orders/nextstage/' + orderId,
        success: function () {
            // Handle success (e.g., update UI, show a message)
            alert('Order canceled successfully.');
            // Redirect to /orders page
            window.location.href = '/orders';
        },
        error: function (error) {
            // Handle error (e.g., show an error message)
            alert('Error cancelling order: ' + error.responseText);
        }
    });
}

// Function to make an AJAX request to advance the order stage
function nextStage(orderId) {
    $.ajax({
        type: 'POST',
        url: '/api/orders/nextstage/' + orderId,
        success: function (data) {
            // Handle success (e.g., update UI, show a message)
            alert(data.Message);
        },
        error: function (error) {
            // Handle error (e.g., show an error message)
            alert('Error advancing order stage: ' + error.responseText);
        }
    });
}