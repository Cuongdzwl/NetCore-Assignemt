
function getURL() {
    return window.location.href ="/Orders";
}
function redirectToLogin() {
    window.location.href = '/Identity/Account/Login';
}
function cancelOrder(orderId) {
    let orderIdString = orderId
    $.ajax({
        url: `/api/orders/cancel/${orderIdString}`,
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        success: function (response) {
            console.log('Order canceled:', response);

            window.location.href = '/orders';
        },
        error: function (error) {
            console.error('Error canceling order:', error);

            if (error.status === 401) {
                redirectToLogin(getURL());
            }
        }
    });
}


function nextStage(orderId) {
    console.log('orderId:', orderId);
    $.ajax({
        url: `/api/orders/nextstage/${orderId}`,
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        success: function (response) {
            console.log('Order status changed:', response.Message);

            window.location.href = '/orders';
        },
        error: function (error) {
            console.error('Error changing order status:', error);

            if (error.status === 401) {
                redirectToLogin(getURL());
            }
        }
    });
}
