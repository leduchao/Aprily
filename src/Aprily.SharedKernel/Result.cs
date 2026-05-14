namespace Aprily.SharedKernel;

public class Result
{
    public bool IsSuccess { get; set; }
    public bool IsFailure => !IsSuccess;

    public Error Error { get; set; } = default!;

    protected Result(bool isSuccess, Error error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, Error.None);
    public static Result Failure(Error error) => new(false, error);
}

public class Result<TData> : Result
{
    public TData? Data { get; set; }

    private Result(bool isSuccess, TData? data, Error error) : base(isSuccess, error)
    {
        Data = data;
    }

    public static Result<TData> Success(TData value) => new(true, value, Error.None);
    public static new Result<TData> Failure(Error error) => new(false, default, error);
}
