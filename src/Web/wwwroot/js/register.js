$(function () {
    initAuthForm({
        formId: '#registerForm',
        btnId: '#registerBtn',
        errorBoxId: 'registerErrorBox',
        url: '/api/account/register',
        successRedirect: '/Account/Login?registered=1',
        getData: function () {
            return {
                name: $('#Input_Name').val().trim(),
                surname: $('#Input_Surname').val().trim(),
                email: $('#Input_Email').val().trim(),
                password: $('#Input_Password').val()
            };
        }
    });
});