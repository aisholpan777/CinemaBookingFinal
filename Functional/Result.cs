
using System;
using System.Collections.Generic;

namespace CinemaBookingFinal;

public abstract record Result<T>
{
    protected Result() { }

    public sealed record Ok(T Value) : Result<T>;

    public sealed record Fail(string Error) : Result<T>;
}

public static class Result
{
    public static Result<T> Ok<T>(T value)
    {
        return new Result<T>.Ok(value);
    }

    public static Result<T> Fail<T>(string error)
    {
        return new Result<T>.Fail(error);
    }

    public static Result<U> Map<T, U>(
        this Result<T> source,
        Func<T, U> function)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(function);

        return source switch
        {
            Result<T>.Ok ok =>
                Ok(function(ok.Value)),

            Result<T>.Fail fail =>
                Fail<U>(fail.Error),

            _ => throw new InvalidOperationException(
                "Unknown Result case")
        };
    }

    public static Result<U> Bind<T, U>(
        this Result<T> source,
        Func<T, Result<U>> function)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(function);

        return source switch
        {
            Result<T>.Ok ok =>
                function(ok.Value),

            Result<T>.Fail fail =>
                Fail<U>(fail.Error),

            _ => throw new InvalidOperationException(
                "Unknown Result case")
        };
    }
}

public abstract record Maybe<T>
{
    private Maybe() { }

    public sealed record Some(T Value) : Maybe<T>;

    public sealed record None : Maybe<T>;
}

public static class MaybeExtensions
{
    public static Maybe<T> Find<T>(
        this IEnumerable<T> items,
        Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(predicate);

        foreach (var item in items)
        {
            if (predicate(item))
                return new Maybe<T>.Some(item);
        }

        return new Maybe<T>.None();
    }
}
