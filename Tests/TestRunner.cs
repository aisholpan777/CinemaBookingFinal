
using System.Collections.Immutable;

namespace CinemaBookingFinal;

public static class TestRunner
{
    private static readonly DateTime Now = new(2027, 1, 1, 12, 0, 0);

    private static CinemaState State() => new(
        ImmutableArray.Create(
            new Movie(1, "A", "Drama", 100),
            new Movie(2, "B", "Comedy", 90)),
        ImmutableArray.Create(
            new Session(10, 1, Now.AddDays(1), 2, 10000),
            new Session(20, 2, Now.AddDays(2), 2, 20000)),
        ImmutableArray<Booking>.Empty);

    private static void Check(bool condition, string message)
    {
        if (!condition)
            throw new Exception(message);
    }

    public static int Run()
    {
        var tests = new (string, Action)[]
        {
            ("Factory accepts valid input", () =>
                Check(ValidatedSelection.Create(State(),
                    new BookingInput(10, 1, "adult"))
                    is Result<ValidatedSelection>.Ok, "factory")),

            ("Factory rejects missing session", () =>
                Check(ValidatedSelection.Create(State(),
                    new BookingInput(99, 1, "adult"))
                    is Result<ValidatedSelection>.Fail, "missing")),

            ("Factory rejects seat boundary", () =>
                Check(ValidatedSelection.Create(State(),
                    new BookingInput(10, 3, "adult"))
                    is Result<ValidatedSelection>.Fail, "seat")),

            ("Factory rejects customer type", () =>
                Check(ValidatedSelection.Create(State(),
                    new BookingInput(10, 1, "other"))
                    is Result<ValidatedSelection>.Fail, "type")),

            ("Factory rejects missing input", () =>
                Check(ValidatedSelection.Create(State(), null)
                    is Result<ValidatedSelection>.Fail, "null")),

            ("Policies differ", () =>
            {
                var a = Pricing.QuoteBooking(State(),
                    new BookingInput(10, 1, "adult"),
                    Pricing.CurryPolicy(0), 0);

                var b = Pricing.QuoteBooking(State(),
                    new BookingInput(10, 1, "adult"),
                    Pricing.CurryPolicy(10), 0);

                Check(
                    a is Result<Quote>.Ok
                    { Value.TotalCents: 10000 } &&
                    b is Result<Quote>.Ok
                    { Value.TotalCents: 9000 },
                    "policy");
            }),

            ("Student discount", () =>
                Check(Pricing.QuoteBooking(State(),
                    new BookingInput(10, 1, "student"),
                    Pricing.CurryPolicy(0), 0)
                    is Result<Quote>.Ok
                    { Value.TotalCents: 8000 }, "discount")),

            ("Empty reports", () =>
                Check(
                    Queries.Revenue(State()).Length == 0 &&
                    Queries.Popularity(State()).Length == 0,
                    "empty")),

            ("Extensions", () =>
            {
                var b = new Booking(1, 10, 1, "adult",
                    10000, new BookingStatus.Confirmed(Now));

                Check(
                    new[] { b }.Active().ForSession(10).Count() == 1 &&
                    new[] { b }.ForSession(20).Count() == 0,
                    "extensions");
            }),

            ("Map and Bind success", () =>
            {
                Result<int> r = Result.Ok(2);

                Check(
                    r.Map(x => x + 1)
                     .Bind(x => Result.Ok(x * 2))
                    is Result<int>.Ok { Value: 6 },
                    "composition");
            }),

            ("Map and Bind skip failure", () =>
            {
                int called = 0;
                Result<int> r = Result.Fail<int>("failed");

                var output = r
                    .Map(x => { called++; return x; })
                    .Bind(x =>
                    {
                        called++;
                        return Result.Ok(x);
                    });

                Check(called == 0 &&
                    output is Result<int>.Fail,
                    "failure propagation");
            }),

            ("Maybe found and missing", () =>
                Check(
                    State().Movies.Find(x => x.Id == 1)
                        is Maybe<Movie>.Some &&
                    State().Movies.Find(x => x.Id == 99)
                        is Maybe<Movie>.None,
                    "lookup")),

            ("Booking and duplicate rejected", () =>
            {
                var q = Pricing.QuoteBooking(State(),
                    new BookingInput(10, 1, "adult"),
                    Pricing.CurryPolicy(0), 0);

                Check(q is Result<Quote>.Ok, "quote");

                var quote = ((Result<Quote>.Ok)q).Value;
                var booked = BookingService.Confirm(
                    State(), quote, Now);

                Check(booked is Result<CinemaState>.Ok, "book");

                var newState =
                    ((Result<CinemaState>.Ok)booked).Value;

                Check(
                    BookingService.Confirm(newState, quote, Now)
                        is Result<CinemaState>.Fail &&
                    State().Bookings.IsEmpty,
                    "duplicate");
            }),

            ("Cancellation and repeat", () =>
            {
                var q = ((Result<Quote>.Ok)
                    Pricing.QuoteBooking(State(),
                        new BookingInput(10, 1, "adult"),
                        Pricing.CurryPolicy(0), 0)).Value;

                var booked = ((Result<CinemaState>.Ok)
                    BookingService.Confirm(State(), q, Now)).Value;

                var cancelled = BookingService.Cancel(
                    booked, 1, Now);

                Check(cancelled is Result<CinemaState>.Ok,
                    "cancel");

                Check(
                    BookingService.Cancel(
                        ((Result<CinemaState>.Ok)cancelled).Value,
                        1, Now)
                    is Result<CinemaState>.Fail,
                    "repeat");
            }),

            ("Revenue and popularity ties", () =>
            {
                var s = State() with
                {
                    Bookings = ImmutableArray.Create(
                        new Booking(1, 10, 1, "adult",
                            10000, new BookingStatus.Confirmed(Now)),
                        new Booking(2, 20, 1, "adult",
                            10000, new BookingStatus.Confirmed(Now)))
                };

                Check(
                    Queries.Revenue(s)
                        .Select(x => x.Movie)
                        .SequenceEqual(new[] { "A", "B" }) &&
                    Queries.Popularity(s)
                        .Select(x => x.Movie)
                        .SequenceEqual(new[] { "A", "B" }),
                    "ties");
            }),

            ("Lazy processing and snapshot", () =>
            {
                var s = State();
                var query = Queries.Upcoming(s, Now);
                var snapshot = query.ToArray();

                Check(snapshot.Length == 2 &&
                    snapshot[0].Id == 10,
                    "lazy snapshot");
            }),

            ("Missing file", () =>
                Check(
                    JsonStore.Load(Path.Combine(
                        Path.GetTempPath(),
                        Guid.NewGuid() + ".json"))
                    is Result<CinemaState>.Fail,
                    "missing file")),

            ("Malformed file", () =>
            {
                var path = Path.Combine(
                    Path.GetTempPath(),
                    Guid.NewGuid() + ".json");

                try
                {
                    File.WriteAllText(path, "{broken");

                    Check(JsonStore.Load(path)
                        is Result<CinemaState>.Fail,
                        "malformed");
                }
                finally
                {
                    File.Delete(path);
                }
            }),

            ("JSON round trip", () =>
            {
                var path = Path.Combine(
                    Path.GetTempPath(),
                    Guid.NewGuid() + ".json");

                try
                {
                    Check(JsonStore.Save(path, State())
                        is Result<string>.Ok, "save");

                    Check(JsonStore.Load(path)
                        is Result<CinemaState>.Ok, "load");
                }
                finally
                {
                    File.Delete(path);
                }
            }),

            ("Native real call", () =>
                Check(
                    new NativeFeeCalculator()
                        .Calculate(10000, 500)
                    is Result<long>.Ok { Value: 500 },
                    "native fee")),

            ("Native failure adapter", () =>
                Check(
                    new FailingFeeCalculator()
                        .Calculate(10000, 500)
                    is Result<long>.Fail,
                    "adapter")),

            ("Native checkout workflow", () =>
            {
                var result = Checkout.BookWithNativeFee(
                    State(),
                    new BookingInput(10, 1, "adult"),
                    Pricing.CurryPolicy(0),
                    new NativeFeeCalculator(),
                    500,
                    Now);

                Check(result is Result<CinemaState>.Ok
                    { Value.Bookings.Length: 1 },
                    "checkout");

                Check(
                    ((Result<CinemaState>.Ok)result)
                        .Value.Bookings[0].PaidCents == 10500,
                    "total");
            }),

            ("Native failure preserves state", () =>
            {
                var s = State();

                Check(
                    Checkout.BookWithNativeFee(
                        s,
                        new BookingInput(10, 1, "adult"),
                        Pricing.CurryPolicy(0),
                        new FailingFeeCalculator(),
                        500,
                        Now)
                    is Result<CinemaState>.Fail &&
                    s.Bookings.IsEmpty,
                    "preserve");
            }),

            ("Add session validation", () =>
            {
                var s = State();

                Check(
                    BookingService.AddSession(
                        s,
                        new Session(30, 1,
                            Now.AddDays(3), 5, 2000))
                    is Result<CinemaState>.Ok,
                    "add");

                Check(
                    BookingService.AddSession(
                        s,
                        new Session(10, 1,
                            Now.AddDays(3), 5, 2000))
                    is Result<CinemaState>.Fail,
                    "duplicate session");
            }),

            ("Reject negative quote", () =>
            {
                var quote = new Quote(
                    10, 1, "adult", -100, 0, -100);

                Check(
                    BookingService.Confirm(State(), quote, Now)
                        is Result<CinemaState>.Fail,
                    "negative quote");
            }),

            ("Reject inconsistent quote total", () =>
            {
                var quote = new Quote(
                    10, 1, "adult", 10000, 500, 10000);

                Check(
                    BookingService.Confirm(State(), quote, Now)
                        is Result<CinemaState>.Fail,
                    "inconsistent total");
            }),

            ("Reject discount over 100 percent", () =>
            {
                bool rejected = false;

                try
                {
                    Pricing.ConfigureDiscount(120);
                }
                catch (ArgumentOutOfRangeException)
                {
                    rejected = true;
                }

                Check(rejected, "invalid discount");
            }),

            ("Reject booking after session start", () =>
            {
                var quote = new Quote(
                    10, 1, "adult", 10000, 0, 10000);

                Check(
                    BookingService.Confirm(
                        State(), quote, Now.AddDays(2))
                    is Result<CinemaState>.Fail,
                    "past session");
            }),

            ("Valid quote remains bookable", () =>
            {
                var result = Pricing.QuoteBooking(
                    State(),
                    new BookingInput(10, 2, "student"),
                    Pricing.CurryPolicy(0),
                    500);

                Check(result is Result<Quote>.Ok,
                    "quote creation");

                var quote = ((Result<Quote>.Ok)result).Value;

                Check(
                    BookingService.Confirm(State(), quote, Now)
                    is Result<CinemaState>.Ok,
                    "valid booking");
            }),
            ("Native fee rounds up", () =>
{
    var result = new NativeFeeCalculator()
        .Calculate(1001, 500);

    Check(
        result is Result<long>.Ok { Value: 51 },
        "native rounding");
}),

("Native fee handles maximum value", () =>
{
    var result = new NativeFeeCalculator()
        .Calculate(long.MaxValue, 10000);

    Check(
        result is Result<long>.Ok
        { Value: long.MaxValue },
        "native maximum");
}),

("Native fee rejects invalid rate", () =>
{
    var calculator = new NativeFeeCalculator();

    Check(
        calculator.Calculate(10000, -1)
            is Result<long>.Fail &&
        calculator.Calculate(10000, 10001)
            is Result<long>.Fail,
        "invalid native rate");
}),

("Available seats after booking", () =>
{
    var s = State() with
    {
        Bookings = ImmutableArray.Create(
            new Booking(
                1,
                10,
                1,
                "adult",
                10000,
                new BookingStatus.Confirmed(Now)))
    };

    var seats = Queries.AvailableSeats(s, 10);

    Check(
        seats.SequenceEqual(new[] { 2 }),
        "available seats");
}),

("Invalid JSON data rejected", () =>
{
    var path = Path.Combine(
        Path.GetTempPath(),
        Guid.NewGuid() + ".json");

    try
    {
        File.WriteAllText(
            path,
            "{\"Movies\":[],\"Sessions\":[],\"Bookings\":[{\"Id\":1,\"SessionId\":99,\"Seat\":1,\"CustomerType\":\"adult\",\"PaidCents\":100,\"Status\":\"Confirmed\",\"At\":\"2027-01-01T12:00:00\"}]}");

        Check(
            JsonStore.Load(path)
                is Result<CinemaState>.Fail,
            "invalid JSON data");
    }
    finally
    {
        File.Delete(path);
    }
})
        };

        int passed = 0;

        foreach (var (name, test) in tests)
        {
            try
            {
                test();
                Console.WriteLine("PASS " + name);
                passed++;
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "FAIL " + name + ": " + e.Message);
            }
        }

        Console.WriteLine(
            $"TOTAL: {passed} passed, {tests.Length - passed} failed");

        return passed == tests.Length ? 0 : 1;
    }
}
