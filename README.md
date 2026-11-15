# Cinema Booking System

A C# .NET 8 console application developed as a four-student final project for the Introduction to Functional Programming course.

The application allows cinema customers and staff to browse movies, search and filter movies, manage sessions, check seat availability, calculate ticket prices, confirm and cancel bookings, and generate reports.

The project demonstrates functional programming principles, including pure functions, immutable data structures, LINQ, higher-order functions, currying, pattern matching, explicit error handling, lazy evaluation, JSON persistence, and native C integration.

All monetary values are stored as integer cents (100 cents = 1 KZT).

## 1. Team Members and Responsibilities

| Student | Name | Main Responsibility |
|---|---|---|
| Student 1 | Aisholpan | Movies, Sessions & Console Interface |
| Student 2 | Dilnaz | Booking, State Validation & JSON Persistence |
| Student 3 | Zhuldyzay | Pricing, Functional Composition & Error Handling |
| Student 4 | Zhasmin | LINQ Reports, Extension Methods & Native C Integration |

### Student 1 — Aisholpan

**Movies, Sessions & Console Interface**

Assigned responsibilities:

- Implement and explain movie browsing and title search.
- Filter movies by genre.
- Display upcoming cinema sessions.
- Validate and add new cinema sessions.
- Connect domain operations to the console interface.
- Handle console input and display operation results.
- Test movie filtering, upcoming sessions, and session validation.

Main components:

- `Program.cs`
- `Movie` and `Session` models in `Models/Domain.cs`
- `Queries.SearchMovies()`
- `Queries.ByGenre()`
- `Queries.Upcoming()`
- `BookingService.AddSession()`

Functional programming concepts:

- LINQ filtering and sorting
- Immutable collections
- Lazy evaluation
- `yield return`
- Pure query functions
- Input validation

### Student 2 — Dilnaz

**Booking, State Validation & JSON Persistence**

Assigned responsibilities:

- Validate booking requests.
- Check seat availability.
- Prevent duplicate bookings.
- Confirm valid bookings.
- Cancel existing bookings.
- Maintain immutable application state.
- Load and save application data using JSON.
- Validate loaded data and handle file errors.
- Test booking transitions and JSON persistence.

Main components:

- `ValidatedSelection.Create()`
- `BookingService.Confirm()`
- `BookingService.Cancel()`
- `Queries.AvailableSeats()`
- `BookingStatus`
- `Persistence/JsonStore.cs`

Functional programming concepts:

- Immutable state transitions
- Smart constructors
- Pattern matching
- Explicit success and failure results
- Safe operation sequencing
- Resource disposal

### Student 3 — Zhuldyzay

**Pricing, Functional Composition & Error Handling**

Assigned responsibilities:

- Implement ticket price calculations.
- Apply customer discounts.
- Configure standard and promotional policies.
- Implement the three-stage pricing pipeline.
- Use higher-order functions for configurable pricing.
- Implement currying and partial application.
- Compose operations with `Map()` and `Bind()`.
- Represent failures with `Result<T>` and missing values with `Maybe<T>`.
- Test pricing rules, pipeline composition, and failure propagation.

Main components:

- `Services/Pricing.cs`
- `Services/Checkout.cs`
- `Functional/Result.cs`
- `Pricing.CurryPolicy()`
- `Pricing.ApplyPolicy()`
- `Pricing.BuildQuote()`
- `Pricing.QuoteBooking()`

Functional programming concepts:

- Pure functions
- Higher-order functions
- Currying
- Partial application
- Function composition
- `Result<T>` and `Maybe<T>`
- `Map()` and `Bind()`

### Student 4 — Zhasmin

**LINQ Reports, Extension Methods & Native C Integration**

Assigned responsibilities:

- Generate revenue reports grouped by movie.
- Generate popularity reports grouped by movie.
- Implement reusable extension methods.
- Apply LINQ filtering, joining, grouping, aggregation, and sorting.
- Implement native C booking fee calculation.
- Connect the native library to C# using P/Invoke.
- Handle native library loading and calculation failures.
- Test LINQ reports and native integration.

Main components:

- `Queries.Revenue()`
- `Queries.Popularity()`
- `BookingExtensions.Active()`
- `BookingExtensions.ForSession()`
- `Native/NativeFee.cs`
- `Native/cinemafee.c`
- `Native/build-macos.sh`
- `Native/build-linux.sh`

Functional programming concepts:

- LINQ queries
- Extension methods
- Grouping and aggregation
- Deterministic ordering
- Native interoperability
- Explicit error handling

### Shared Responsibilities

All four students participate in:

- Integration testing and debugging.
- Reviewing the complete application.
- Git repository management.
- Group documentation.
- Preparing individual contribution reports.
- Demonstrating the application during the final defense.

Each student must contribute meaningful application logic and automated tests, understand the overall application flow, and explain their own implementation.

The division above defines the agreed