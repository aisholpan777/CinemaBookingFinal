
namespace CinemaBookingFinal;

public static class BookingService
{
    public static Result<CinemaState> Confirm(
        CinemaState state,
        Quote quote,
        DateTime now)
    {
        if (state is null)
            return Result.Fail<CinemaState>("Missing cinema state");

        if (quote is null)
            return Result.Fail<CinemaState>("Invalid quote");

        
        if (quote.DiscountedCents < 0 ||
            quote.FeeCents < 0 ||
            quote.TotalCents < 0 ||
            quote.DiscountedCents > long.MaxValue - quote.FeeCents ||
            quote.TotalCents != quote.DiscountedCents + quote.FeeCents)
        {
            return Result.Fail<CinemaState>("Invalid quote total");
        }

        var session = state.Sessions.FirstOrDefault(
            s => s.Id == quote.SessionId);

        if (session is null)
            return Result.Fail<CinemaState>("Session not found");

        if (session.StartsAt <= now)
            return Result.Fail<CinemaState>(
                "Session has already started");

        
        return ValidatedSelection.Create(
                state,
                new BookingInput(
                    quote.SessionId,
                    quote.Seat,
                    quote.CustomerType))
            .Bind(selection =>
            {
                
                if (quote.CustomerType != selection.CustomerType)
                    return Result.Fail<CinemaState>(
                        "Invalid quote customer type");

                
                if (state.Bookings.Any(b => b.Id <= 0))
                    return Result.Fail<CinemaState>(
                        "Invalid existing booking ID");

                var ids = state.Bookings.Select(b => b.Id).ToArray();

                if (ids.Distinct().Count() != ids.Length)
                    return Result.Fail<CinemaState>(
                        "Duplicate existing booking ID");

                int maxId = state.Bookings.Length == 0
                    ? 0
                    : state.Bookings.Max(b => b.Id);

                if (maxId == int.MaxValue)
                    return Result.Fail<CinemaState>(
                        "Booking ID limit reached");

                int nextId = maxId + 1;

                var booking = new Booking(
                    nextId,
                    selection.SessionId,
                    selection.Seat,
                    selection.CustomerType,
                    quote.TotalCents,
                    new BookingStatus.Confirmed(now));

                
                return Result.Ok(state with
                {
                    Bookings = state.Bookings.Add(booking)
                });
            });
    }

    public static Result<CinemaState> Cancel(
        CinemaState state,
        int bookingId,
        DateTime now)
    {
        if (state is null)
            return Result.Fail<CinemaState>("Missing cinema state");

        if (bookingId <= 0)
            return Result.Fail<CinemaState>("Invalid booking ID");

        return state.Bookings.Find(
            b => b.Id == bookingId) switch
        {
            Maybe<Booking>.None =>
                Result.Fail<CinemaState>(
                    "Booking not found"),

            Maybe<Booking>.Some
            {
                Value.Status: BookingStatus.Cancelled
            } =>
                Result.Fail<CinemaState>(
                    "Booking already cancelled"),

            Maybe<Booking>.Some
            {
                Value: var booking
            } =>
                CancelConfirmed(state, booking, now),

            _ => Result.Fail<CinemaState>(
                "Unknown booking state")
        };
    }

    private static Result<CinemaState> CancelConfirmed(
        CinemaState state,
        Booking booking,
        DateTime now)
    {
        if (booking.Status is not BookingStatus.Confirmed confirmed)
            return Result.Fail<CinemaState>(
                "Booking is not confirmed");

        var session = state.Sessions.FirstOrDefault(
            s => s.Id == booking.SessionId);

        if (session is null)
            return Result.Fail<CinemaState>(
                "Session not found");

        if (now < confirmed.At)
            return Result.Fail<CinemaState>(
                "Cancellation cannot precede confirmation");

        if (session.StartsAt <= now)
            return Result.Fail<CinemaState>(
                "Cannot cancel after session start");

        var cancelledBooking = booking with
        {
            Status = new BookingStatus.Cancelled(now)
        };

        
        return Result.Ok(state with
        {
            Bookings = state.Bookings.Replace(
                booking,
                cancelledBooking)
        });
    }

    public static Result<CinemaState> AddSession(
        CinemaState state,
        Session session)
    {
        if (state is null)
            return Result.Fail<CinemaState>("Missing cinema state");

        if (session is null)
            return Result.Fail<CinemaState>("Missing session");

        if (session.Id <= 0 ||
            session.MovieId <= 0 ||
            session.Seats <= 0 ||
            session.BasePriceCents < 0)
        {
            return Result.Fail<CinemaState>(
                "Invalid session data");
        }

        if (session.StartsAt == default)
            return Result.Fail<CinemaState>(
                "Invalid session date");

        if (!state.Movies.Any(
            m => m.Id == session.MovieId))
        {
            return Result.Fail<CinemaState>(
                "Movie not found");
        }

        if (state.Sessions.Any(
            s => s.Id == session.Id))
        {
            return Result.Fail<CinemaState>(
                "Duplicate session ID");
        }

        return Result.Ok(state with
        {
            Sessions = state.Sessions.Add(session)
        });
    }
}
