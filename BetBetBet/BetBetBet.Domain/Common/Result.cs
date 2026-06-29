using System.Diagnostics.CodeAnalysis;

namespace BetBetBet.Domain.Common;

public sealed class Result<T>
{
    public T? Value { get; }
    public Error? Error { get; }

    [MemberNotNullWhen(true,  nameof(Value))]
    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess => Error is null;

    [MemberNotNullWhen(false, nameof(Value))]
    [MemberNotNullWhen(true,  nameof(Error))]
    public bool IsFailure => !IsSuccess;

    private Result(T value)    { Value = value; }
    private Result(Error error) { Error = error; }

    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);

    public static implicit operator Result<T>(T value)    => Success(value);
    public static implicit operator Result<T>(Error error) => Failure(error);
}
