
using System;
using System.Collections.Immutable;
using System.Linq;

namespace CinemaBookingFinal;

public sealed record Movie(
    int Id,
    string Title,
    string Genre,
    int DurationMinutes);

public sealed record Session(
    int Id,
    int MovieId,
    DateTime StartsAt,
    int Seats,
    long BasePriceCents);

public abstract record BookingStatus
{
    protected BookingStatus() { }

    public sealed record Confirmed(DateTime At)
        : BookingStatus;

    public sealed record Cancelled(DateTime At)
        : BookingStatus;
}

public sealed record Booking(
    int Id,
    int SessionId,
    int Seat,
    string CustomerType,
    long PaidCents,
    BookingStatus Status);

public sealed record CinemaState(
    ImmutableArray<Movie> Movies,
    ImmutableArray<Session> Sessions,
    ImmutableArray<Booking> Bookings);

public sealed record Quote(
    int SessionId,
    int Seat,
    string CustomerType,
    long DiscountedCents,
    long FeeCents,
    long TotalCents);

public sealed record RevenueRow(
    string Movie,
    int Tickets,
    long TotalCents);

public sealed record PopularityRow(
    string Movie,
    int Tickets);

public sealed record BookingInput(
    int SessionId,
    int Seat,
    string CustomerType);

public sealed record PricedSelection(
    ValidatedSelection Selection,
    long DiscountedCents);


public sealed class ValidatedSelection
{
    public int SessionId { get; }
    public int Seat { get; }
    public string CustomerType { get; }
    public long BasePriceCents { get; }

    private ValidatedSelection(
        int sessionId,
        int seat,
        string customerType,
        long basePriceCents)
    {
        SessionId = sessionId;
        Seat = seat;
        CustomerType = customerType;
        BasePriceCents = basePriceCents;
    }

    public static Result<ValidatedSelection> Create(
        CinemaState state,
        BookingInput? input)
    {
        if (state is null)
            return Result.Fail<ValidatedSelection>(
                "Missing cinema state");

        if (input is null)
            return Result.Fail<ValidatedSelection>(
                "Missing booking input");

        if (input.SessionId <= 0)
            return Result.Fail<ValidatedSelection>(
                "Invalid session ID");

        if (input.Seat <= 0)
            return Result.Fail<ValidatedSelection>(
                "Seat outside valid range");

        var session = state.Sessions.FirstOrDefault(
            s => s.Id == input.SessionId);

        if (session is null)
            return Result.Fail<ValidatedSelection>(
                "Session not found");

        if (session.Seats <= 0)
            return Result.Fail<ValidatedSelection>(
                "Invalid session capacity");

        if (input.Seat > session.Seats)
            return Result.Fail<ValidatedSelection>(
                "Seat outside valid range");

        if (session.BasePriceCents < 0)
            return Result.Fail<ValidatedSelection>(
                "Invalid session price");

        var customerType = input.CustomerType?
            .Trim()
            .ToLowerInvariant();

        if (customerType is not ("adult" or "student" or "child"))
            return Result.Fail<ValidatedSelection>(
                "Unknown customer type");

        bool seatAlreadyBooked = state.Bookings.Any(
            b => b.SessionId == input.SessionId &&
                 b.Seat == input.Seat &&
                 b.Status is BookingStatus.Confirmed);

        if (seatAlreadyBooked)
            return Result.Fail<ValidatedSelection>(
                "Seat already booked");

        return Result.Ok(
            new ValidatedSelection(
                session.Id,
                input.Seat,
                customerType,
                session.BasePriceCents));
    }
}
