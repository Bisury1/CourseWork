namespace WebApp.Application.Common;

public readonly struct Result<TResult, TError>(TResult value, TError error, bool success)
{
    public bool IsSuccess { get; } = success;
    public TResult Value { get; } = value;
    public TError? Error { get; } = error;

    public static Result<TResult, TError?> Ok(TResult v)
    {
        return new(v, default, true);
    }

    public static Result<TResult?, TError> Err(TError e)
    {
        return new Result<TResult?, TError>(default, e, false);
    }
    
    public static implicit operator Result<TResult, TError>(TResult v) => new(v, default!, true);
    public static implicit operator Result<TResult, TError>(TError e) => new(default!, e, false);
    
    public TR Match<TR>(
        Func<TResult, TR> success,
        Func<TError, TR> failure) =>
        IsSuccess ? success(Value) : failure(Error);
}