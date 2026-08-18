$(function () {
    const urlParams = new URLSearchParams(window.location.search);
    if (urlParams.get('registered') === '1') {
        $('#registeredSuccess').show();

        window.history.replaceState({}, document.title, window.location.pathname);
    }

    initAuthForm({
        formId: '#loginForm',
        btnId: '#loginBtn',
        errorBoxId: 'loginErrorBox',
        url: '/api/account/login',
        successRedirect: '/Users',
        getData: function () {
            return {
                email: $('#Input_Email').val().trim(),
                password: $('#Input_Password').val(),
                rememberMe: $('#Input_RememberMe').is(':checked')
            };
        }
    });
});