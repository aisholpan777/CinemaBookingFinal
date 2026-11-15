
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace CinemaBookingFinal;

public static class BookingExtensions
{
    public static IEnumerable<Booking> Active(
        this IEnumerable<Booking> bookings)
    {
        ArgumentNullException.ThrowIfNull(bookings);

        return bookings.Where(
            b => b.Status is BookingStatus.Confirmed);
    }

    public static IEnumerable<Booking> ForSession(
        this IEnumerable<Booking> bookings,
        int sessionId)
    {
        ArgumentNullException.ThrowIfNull(bookings);

        return bookings.Where(
            b => b.SessionId == sessionId);
    }
}

public static class Queries
{
    public static ImmutableArray<Movie> SearchMovies(
        CinemaState state,
        string term)
    {
        ArgumentNullException.ThrowIfNull(state);

        var searchTerm = term?.Trim() ?? "";

        return state.Movies
            .Where(m => m.Title.Contains(
                searchTerm,
                StringComparison.OrdinalIgnoreCase))
            .OrderBy(m => m.Title)
            .ThenBy(m => m.Id)
            .ToImmutableArray();
    }

    public static ImmutableArray<Movie> ByGenre(
        CinemaState state,
        string genre)
    {
        ArgumentNullException.ThrowIfNull(state);

        var searchGenre = genre?.Trim() ?? "";

        return state.Movies
            .Where(m => m.Genre.Equals(
                searchGenre,
                StringComparison.OrdinalIgnoreCase))
            .OrderBy(m => m.Title)
            .ThenBy(m => m.Id)
            .ToImmutableArray();
    }

    public static IEnumerable<Session> Upcoming(
        CinemaState state,
        DateTime from)
    {
        ArgumentNullException.ThrowIfNull(state);

        return UpcomingIterator(state, from);
    }

    private static IEnumerable<Session> UpcomingIterator(
        CinemaState state,
        DateTime from)
    {
        foreach (var session in state.Sessions
            .OrderBy(s => s.StartsAt)
            .ThenBy(s => s.Id))
        {
            if (session.StartsAt >= from)
                yield return session;
        }
    }

    public static Result<ImmutableArray<int>> AvailableSeatsChecked(
        CinemaState state,
        int sessionId)
    {
        if (state is null)
            return Result.Fail<ImmutableArray<int>>(
                "Missing cinema state");

        if (sessionId <= 0)
            return Result.Fail<ImmutableArray<int>>(
                "Invalid session ID");

        var session = state.Sessions.FirstOrDefault(
            s => s.Id == sessionId);

        if (session is null)
            return Result.Fail<ImmutableArray<int>>(
                "Session not found");

        if (session.Seats <= 0)
            return Result.Fail<ImmutableArray<int>>(
                "Invalid session capacity");

        var occupied = state.Bookings
            .Active()
            .ForSession(sessionId)
            .Select(b => b.Seat)
            .ToHashSet();

        var available = Enumerable.Range(1, session.Seats)
            .Where(seat => !occupied.Contains(seat))
            .ToImmutableArray();

        return Result.Ok(available);
    }

    public static ImmutableArray<int> AvailableSeats(
        CinemaState state,
        int sessionId)
    {
        if (state is null)
            return ImmutableArray<int>.Empty;

        var session = state.Sessions.FirstOrDefault(
            s => s.Id == sessionId);

        if (session is null || session.Seats <= 0)
            return ImmutableArray<int>.Empty;

        var occupied = state.Bookings
            .Active()
            .ForSession(sessionId)
            .Select(b => b.Seat)
            .ToHashSet();

        return Enumerable.Range(1, session.Seats)
            .Where(seat => !occupied.Contains(seat))
            .ToImmutableArray();
    }

    public static ImmutableArray<RevenueRow> Revenue(
        CinemaState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        return (
            from b in state.Bookings.Active()
            join s in state.Sessions
                on b.SessionId equals s.Id
            join m in state.Movies
                on s.MovieId equals m.Id
            group b by new { m.Id, m.Title } into g
            let total = g.Sum(b => b.PaidCents)
            orderby total descending,
                    g.Key.Title ascending,
                    g.Key.Id ascending
            select new RevenueRow(
                g.Key.Title,
                g.Count(),
                total)
        ).ToImmutableArray();
    }

    public static ImmutableArray<PopularityRow> Popularity(
        CinemaState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        return (
            from b in state.Bookings.Active()
            join s in state.Sessions
                on b.SessionId equals s.Id
            join m in state.Movies
                on s.MovieId equals m.Id
            group b by new { m.Id, m.Title } into g
            let count = g.Count()
            orderby count descending,
                    g.Key.Title ascending,
                    g.Key.Id ascending
            select new PopularityRow(
                g.Key.Title,
                count)
        ).ToImmutableArray();
    }
}
