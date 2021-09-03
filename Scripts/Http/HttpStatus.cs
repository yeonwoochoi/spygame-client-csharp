namespace Http
{
    public enum HttpStatus
    {
        OK = 200,
        CREATED = 201,
        ACCEPTED = 202,
        NO_CONTENT = 204,
        BAD_REQUEST = 400,
        UNAUTHORIZED = 401,
        FORBIDDEN = 403,
        NOT_FOUND = 404,
        METHOD_NOT_ALLOWED = 405,
        CONFLICT = 409,
        TOO_MANY_REQUEST = 429
    }

    public static class HttpStatusExtensions
    {
        public static HttpStatus StringToStatus(string code)
        {
            switch (code)
            {
                case "ok":
                    return HttpStatus.OK;
                case "created":
                    return HttpStatus.CREATED;
                case "accepted":
                    return HttpStatus.ACCEPTED;
                case "no_content":
                    return HttpStatus.NO_CONTENT;
                case "bad_request":
                    return HttpStatus.BAD_REQUEST;
                case "unauthorized":
                    return HttpStatus.UNAUTHORIZED;
                case "forbidden":
                    return HttpStatus.FORBIDDEN;
                case "not_found":
                    return HttpStatus.NOT_FOUND;
                case "method_not_allowed":
                    return HttpStatus.METHOD_NOT_ALLOWED;
                case "conflict":
                    return HttpStatus.CONFLICT;
                case "too_many_request":
                    return HttpStatus.TOO_MANY_REQUEST;
            }

            return HttpStatus.OK;
        }
        
    }
}