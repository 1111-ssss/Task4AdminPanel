namespace Business.Constants;

public class EmailSenderConstants
{
    public const int EMAIL_CONFIRMATION_TOKEN_EXPIRATION_MINUTES = 30;
    public static readonly string EMAIL_SUBJECT = "Email verification - The App";
    public static readonly string EMAIL_BODY = $@"
        <p>Hello!</p>
        <p>Thank you for registering on the App.</p>
        <p>To confirm your email, please click the link below:</p>
        <a href='{{0}}'>Confirm email</a>
        <p>If you did not request this email, please ignore this message.</p>
    ";
}