namespace Namaa.Application.Common.Results;

// Expected business outcomes travel through Result; exceptions remain for unexpected technical failures.
public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
        {
            throw new InvalidOperationException("A successful result cannot contain an error.");
        }

        if (!isSuccess && error == Error.None)
        {
            throw new InvalidOperationException("A failed result must contain an error.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    public static Result<TValue> Failure<TValue>(Error error) => new(default!, false, error);
}

public sealed class Result<TValue> : Result
{
    internal Result(TValue value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        Value = value;
    }

    public TValue Value { get; }
}
