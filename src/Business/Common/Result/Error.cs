using System.Net;

namespace Business.Common.Result;

public record Error(
    HttpStatusCode StatusCode,
    string Code,
    string Message
);