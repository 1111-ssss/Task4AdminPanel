function initAuthForm(options) {
    const $form = $(options.formId);
    const $btn = $(options.btnId);
    const $btnText = $btn.find('#btnText');
    const $btnSpinner = $btn.find('#btnSpinner');

    let $errorBox = $('#' + options.errorBoxId);
    if ($errorBox.length === 0) {
        $errorBox = $('<div id="' + options.errorBoxId + '" class="alert alert-danger mb-3" style="display:none;"></div>');
        $form.prepend($errorBox);
    }

    $form.on('submit', function (e) {
        e.preventDefault();

        if (!$form.valid()) {
            return;
        }

        setLoading(true);

        const data = options.getData();

        $.ajax({
            url: options.url,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            success: function (response) {
                window.location.href = options.successRedirect;
            },
            error: function (xhr) {
                let message = 'An error occurred. Please try again later.';
                if (xhr.responseJSON && xhr.responseJSON.error) {
                    message = xhr.responseJSON.error;
                }
                $errorBox.html(message).show();
            },
            complete: function () {
                setLoading(false);
            }
        });
    });

    function setLoading(isLoading) {
        $btn.prop('disabled', isLoading);
        $btnText.toggleClass('d-none', isLoading);
        $btnSpinner.toggleClass('d-none', !isLoading);
    }
}