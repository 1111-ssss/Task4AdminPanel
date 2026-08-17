using System.Net;
using Data.Common.Result;

namespace Data.Common.Errors;

public static class Errors
{
    public static readonly Error InvalidCredentials = new(HttpStatusCode.BadRequest, "INVALID_CREDENTIALS", "Invalid credentials");
    public static readonly Error EmailAlreadyExists = new(HttpStatusCode.BadRequest, "EMAIL_ALREADY_EXISTS", "User with this email already exists");
    public static readonly Error UserBlocked = new(HttpStatusCode.BadRequest, "USER_BLOCKED", "User is blocked");
    public static readonly Error DatabaseError = new(HttpStatusCode.InternalServerError, "DATABASE_ERROR", "Database error");
    public static readonly Error InvalidEmailToken = new(HttpStatusCode.BadRequest, "INVALID_EMAIL_TOKEN", "Invalid token");
    public static readonly Error UserIsVerifiedOrBlocked = new(HttpStatusCode.BadRequest, "USER_IS_VERIFIED_OR_BLOCKED", "User is already verified or blocked");
    public static readonly Error TokenExpired = new(HttpStatusCode.BadRequest, "TOKEN_EXPIRED", "Token expired");
    public static readonly Error UserNotFound = new(HttpStatusCode.BadRequest, "USER_NOT_FOUND", "User not found");
    public static readonly Error TokenAlreadySentAndValid = new(HttpStatusCode.Forbidden, "TOKEN_ALREADY_SENT_AND_VALID", "Email confirmation token already sent and is still valid");
    public static readonly Error UserAlreadyBlocked = new(HttpStatusCode.BadRequest, "USER_ALREADY_BLOCKED", "User is already blocked");
    public static readonly Error UserNotBlocked = new(HttpStatusCode.BadRequest, "USER_NOT_BLOCKED", "User is not blocked");
    
}