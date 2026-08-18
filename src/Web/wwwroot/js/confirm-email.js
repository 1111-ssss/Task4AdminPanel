$(function () {
    const $status = $('#confirmStatus');
    const params = new URLSearchParams(window.location.search);
    const token = params.get('token');

    if (!token) {
        showError('Confirmation token is missing.');
        return;
    }

    $.ajax({
        url: '/api/account/confirm-email?token=' + encodeURIComponent(token),
        type: 'POST',
        success: function (response) {
            showSuccess('Email confirmed successfully! You can now sign in.');
        },
        error: function (xhr) {
            let message = 'Failed to confirm email. The link may be invalid or expired.';
            if (xhr.responseJSON && xhr.responseJSON.error) {
                message = xhr.responseJSON.error;
            }
            showError(message);
        }
    });

    function showSuccess(text) {
        $status.html(`
            <div class="alert alert-success mb-0">
                <h5 class="alert-heading mb-2">Success!</h5>
                <p class="mb-0">${text}</p>
            </div>
        `);
    }

    function showError(text) {
        $status.html(`
            <div class="alert alert-danger mb-0">
                <h5 class="alert-heading mb-2">Error</h5>
                <p class="mb-0">${text}</p>
            </div>
        `);
    }
});