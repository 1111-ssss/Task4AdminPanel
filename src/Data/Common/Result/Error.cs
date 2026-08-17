using System.Net;

namespace Data.Common.Result;

public record Error(
    HttpStatusCode StatusCode,
    string Code,
    string Message
);