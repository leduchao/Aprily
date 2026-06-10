using Aprily.Backend.Common.Results;

namespace Aprily.Backend.Common.Extensions;

public static class ResultExtension
{
    public static IResult ToHttpResult(this Result result)
    {
        return result.IsSuccess
            ? Microsoft.AspNetCore.Http.Results.Ok(result)
            : Microsoft.AspNetCore.Http.Results.BadRequest(result);
    }

    public static IResult ToHttpResult<TData>(this Result<TData> result)
    {
        return result.IsSuccess
            ? Microsoft.AspNetCore.Http.Results.Ok(result)
            : Microsoft.AspNetCore.Http.Results.BadRequest(result);
    }
}
