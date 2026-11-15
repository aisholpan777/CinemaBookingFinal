
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CinemaBookingFinal;

public static class Program
{
    private static string DataPath =>
        Path.Combine(AppContext.BaseDirectory, "Data", "cinema.json");

    private static readonly IFeeCalculator Native =
        new NativeFeeCalculator();

    public static int Main(string[] args)
    {
        if (args.Contains("--test"))
            return TestRunner.Run();

        var loaded = JsonStore.Load(DataPath);

        if (loaded is Result<CinemaState>.Fail error)
        {
            Console.WriteLine("Error: " + error.Error);
            return 1;
        }

        var state = ((Result<CinemaState>.Ok)loaded).Value;

        while (true)
        {
            Console.WriteLine("\n=== CINEMA BOOKING ===");
            Console.WriteLine("1 Browse movies");
            Console.WriteLine("2 Search movies");
            Console.WriteLine("3 Filter genre");
            Console.WriteLine("4 Upcoming sessions");
            Console.WriteLine("5 Available seats");
            Console.WriteLine("6 Quote ticket");
            Console.WriteLine("7 Book ticket");
            Console.WriteLine("8 Cancel booking");
            Console.WriteLine("9 Revenue report");
            Console.WriteLine("10 Popularity report");
            Console.WriteLine("11 Add session");
            Console.WriteLine("12 Pricing policies");
            Console.WriteLine("0 Exit");
            Console.Write("Choose: ");

            var choice = Console.ReadLine();

            if (choice is null || choice.Trim() == "0")
                return 0;

            try
            {
                switch (choice.Trim())
                {
                    case "1":
                        PrintMovies(state.Movies);
                        break;

                    case "2":
                        Console.Write("Title keyword: ");
                        PrintMovies(Queries.SearchMovies(
                            state,
                            Console.ReadLine() ?? ""));
                        break;

                    case "3":
                        Console.Write("Genre: ");
                        PrintMovies(Queries.ByGenre(
                            state,
                            Console.ReadLine() ?? ""));
                        break;

                    case "4":
                        var upcoming = Queries.Upcoming(
                            state,
                            DateTime.Now).ToArray();

                        if (upcoming.Length == 0)
                        {
                            Console.WriteLine("No upcoming sessions");
                            break;
                        }

                        foreach (var s in upcoming)
                        {
                            var movie = state.Movies.FirstOrDefault(
                                m => m.Id == s.MovieId);

                            Console.WriteLine(
                                $"{s.Id}: {movie?.Title ?? "Unknown movie"} | " +
                                $"{s.StartsAt:g} | " +
                                $"{s.BasePriceCents / 100m:F2} KZT");
                        }
                        break;

                    case "5":
                        var sessionId = ReadInt("Session ID");

                        Print(
                            Queries.AvailableSeatsChecked(
                                state,
                                sessionId),
                            seats => seats.Length == 0
                                ? "No available seats"
                                : "Available seats: " +
                                  string.Join(", ", seats));
                        break;

                    case "6":
                    case "7":
                        var input = ReadBooking();
                        var policy = ChoosePolicy();

                        if (choice.Trim() == "6")
                        {
                            Print(
                                Checkout.QuoteWithNativeFee(
                                    state,
                                    input,
                                    policy,
                                    Native,
                                    500),
                                q =>
                                    $"Price: {q.DiscountedCents / 100m:F2} KZT | " +
                                    $"Fee: {q.FeeCents / 100m:F2} KZT | " +
                                    $"Total: {q.TotalCents / 100m:F2} KZT");
                        }
                        else
                        {
                            var result = Checkout.BookWithNativeFee(
                                state,
                                input,
                                policy,
                                Native,
                                500,
                                DateTime.Now);

                            if (result is Result<CinemaState>.Ok ok)
                            {
                                var saved = JsonStore.Save(
                                    DataPath,
                                    ok.Value);

                                if (saved is Result<string>.Ok)
                                {
                                    state = ok.Value;
                                    Console.WriteLine(
                                        "Booking confirmed, ID " +
                                        state.Bookings[^1].Id);
                                }
                                else
                                {
                                    Print(saved, x => x);
                                }
                            }
                            else
                            {
                                Print(result, _ => "");
                            }
                        }
                        break;

                    case "8":
                        var cancelled = BookingService.Cancel(
                            state,
                            ReadInt("Booking ID"),
                            DateTime.Now);

                        if (cancelled is Result<CinemaState>.Ok done)
                        {
                            var saved = JsonStore.Save(
                                DataPath,
                                done.Value);

                            if (saved is Result<string>.Ok)
                            {
                                state = done.Value;
                                Console.WriteLine("Booking cancelled");
                            }
                            else
                            {
                                Print(saved, x => x);
                            }
                        }
                        else
                        {
                            Print(cancelled, _ => "");
                        }
                        break;

                    case "9":
                        var revenue = Queries.Revenue(state);

                        if (revenue.Length == 0)
                        {
                            Console.WriteLine("No revenue data");
                            break;
                        }

                        foreach (var r in revenue)
                        {
                            Console.WriteLine(
                                $"{r.Movie}: {r.Tickets} tickets, " +
                                $"{r.TotalCents / 100m:F2} KZT");
                        }
                        break;

                    case "10":
                        var popularity = Queries.Popularity(state);

                        if (popularity.Length == 0)
                        {
                            Console.WriteLine("No popularity data");
                            break;
                        }

                        foreach (var r in popularity)
                        {
                            Console.WriteLine(
                                $"{r.Movie}: {r.Tickets} tickets");
                        }
                        break;

                    case "11":
                        var session = new Session(
                            ReadInt("New session ID"),
                            ReadInt("Movie ID"),
                            ReadDate("Start (yyyy-MM-dd HH:mm)"),
                            ReadInt("Seat count"),
                            ReadMoney("Base price KZT"));

                        if (session.StartsAt <= DateTime.Now)
                        {
                            Console.WriteLine(
                                "Error: Session must start in the future");
                            break;
                        }

                        var added = BookingService.AddSession(
                            state,
                            session);

                        if (added is Result<CinemaState>.Ok newState)
                        {
                            var saved = JsonStore.Save(
                                DataPath,
                                newState.Value);

                            if (saved is Result<string>.Ok)
                            {
                                state = newState.Value;
                                Console.WriteLine("Session added");
                            }
                            else
                            {
                                Print(saved, x => x);
                            }
                        }
                        else
                        {
                            Print(added, _ => "");
                        }
                        break;

                    case "12":
                        var example = new BookingInput(
                            ReadInt("Session ID"),
                            ReadInt("Seat"),
                            ReadType());

                        Print(
                            Pricing.QuoteBooking(
                                state,
                                example,
                                Pricing.CurryPolicy(0),
                                0),
                            q =>
                                $"Standard: {q.TotalCents / 100m:F2} KZT");

                        Print(
                            Pricing.QuoteBooking(
                                state,
                                example,
                                Pricing.CurryPolicy(10),
                                0),
                            q =>
                                $"Promotion: {q.TotalCents / 100m:F2} KZT");
                        break;

                    default:
                        Console.WriteLine("Unknown option");
                        break;
                }
            }
            catch (FormatException e)
            {
                Console.WriteLine(
                    "Input error: " + e.Message);
            }
            catch (OverflowException e)
            {
                Console.WriteLine(
                    "Number out of range: " + e.Message);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(
                    "Operation failed: " + e.Message);
            }
        }
    }

    private static void PrintMovies(
        IEnumerable<Movie> movies)
    {
        var found = false;

        foreach (var m in movies)
        {
            found = true;
            Console.WriteLine(
                $"{m.Id}: {m.Title} | " +
                $"{m.Genre} | {m.DurationMinutes} min");
        }

        if (!found)
            Console.WriteLine("No movies found");
    }

    private static void Print<T>(
        Result<T> result,
        Func<T, string> show)
    {
        Console.WriteLine(result switch
        {
            Result<T>.Ok ok => show(ok.Value),
            Result<T>.Fail fail => "Error: " + fail.Error,
            _ => "Unknown result"
        });
    }

    private static string ReadText(string label)
    {
        Console.Write(label + ": ");

        var value = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(value))
            throw new FormatException(
                "Missing " + label);

        return value.Trim();
    }

    private static int ReadInt(string label)
    {
        return int.Parse(
            ReadText(label),
            CultureInfo.InvariantCulture);
    }

    private static long ReadMoney(string label)
    {
        var amount = decimal.Parse(
            ReadText(label),
            CultureInfo.InvariantCulture);

        if (amount < 0)
            throw new FormatException(
                "Price cannot be negative");

        var cents = amount * 100m;

        if (cents != decimal.Truncate(cents))
            throw new FormatException(
                "Price must have at most two decimal places");

        return checked((long)cents);
    }

    private static DateTime ReadDate(string label)
    {
        return DateTime.ParseExact(
            ReadText(label),
            "yyyy-MM-dd HH:mm",
            CultureInfo.InvariantCulture);
    }

    private static string ReadType()
    {
        return ReadText(
            "Customer type (adult/student/child)");
    }

    private static BookingInput ReadBooking()
    {
        return new BookingInput(
            ReadInt("Session ID"),
            ReadInt("Seat"),
            ReadType());
    }

    private static Func<long, long> ChoosePolicy()
    {
        return ReadText(
            "Policy (standard/promo)")
            .ToLowerInvariant() switch
        {
            "standard" => Pricing.CurryPolicy(0),
            "promo" => Pricing.CurryPolicy(10),
            _ => throw new FormatException(
                "Unknown policy")
        };
    }
}
