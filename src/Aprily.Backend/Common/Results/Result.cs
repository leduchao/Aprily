namespace Aprily.Backend.Common.Results;

public class Result
{
    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;

    public Error? Error { get; init; }

    protected Result(bool isSuccess, Error? error = null)
    {
        if (isSuccess && error is not null)
        {
            throw new ArgumentException("Invalid result", nameof(error));
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true);
    public static Result Failure(Error error) => new(false, error);
}

public class Result<TData> : Result
{
    public TData? Data { get; init; }

    private Result(bool isSuccess, TData? data, Error? error = null) : base(isSuccess, error)
    {
        Data = data;
    }

    public static Result<TData> Success(TData value) => new(true, value);
    public static new Result<TData> Failure(Error error) => new(false, default, error);
}
