
# Cinema Booking System

A C# .NET 8 console application developed as a four-student final project for the Introduction to Functional Programming course.

The application allows cinema customers and staff to browse movies, search and filter movies, manage sessions, check seat availability, calculate ticket prices, confirm and cancel bookings, and generate reports.

The project demonstrates functional programming principles, including pure functions, immutable data structures, LINQ, higher-order functions, currying, pattern matching, explicit error handling, lazy evaluation, JSON persistence, and native C integration.

All monetary values are stored as integer cents (100 cents = 1 KZT).

## Git Repository and Submitted Version

**Repository URL:** https://github.com/aisholpan777/CinemaBookingFinal

**Branch:** `main`

**Final Commit SHA:** `e719bd9d8362a7ec44d86517ad36a71fa3634c3e`

All team members use the same repository, branch, and submitted project version.

## 1. Team Members and Responsibilities

| Student | Name | Main Responsibility |
|---|---|---|
| Student 1 | Aisholpan Bolatova | Movies, Sessions, Console Interface, and System Integration |
| Student 2 | Dilnaz Momenzhanova | Booking Management, Domain Validation, and JSON Persistence |
| Student 3 | Zhuldyzay Galymzhankyzy | Pricing, Functional Composition, and Error Handling |
| Student 4 | Zhasmin Askar | LINQ Reports, Extension Methods, and Native C Integration |

### Student 1 — Aisholpan

**Movies, Sessions, Console Interface, and System Integration**

Responsibilities:

- Implement and explain movie browsing and title search.
- Filter movies by genre.
- Display upcoming cinema sessions.
- Validate and add new cinema sessions.
- Connect domain operations to the console interface.
- Handle console input and display operation results.
- Integrate the application's functional modules.
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

**Booking Management, Domain Validation, and JSON Persistence**

Responsibilities:

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
- `Queries.AvailableSeatsChecked()`
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

**Pricing, Functional Composition, and Error Handling**

Responsibilities:

- Implement ticket price calculations.
- Apply customer discounts.
- Configure standard and promotional policies.
- Implement the pricing pipeline.
- Use higher-order functions for configurable pricing.
- Implement currying and partial application.
- Compose operations with `Map()` and `Bind()`.
- Represent failures with `Result<T>` and missing values with `Maybe<T>`.
- Test pricing rules, pipeline composition, and failure propagation.

Main components:

- `Services/Pricing.cs`
- `Services/Checkout.cs`
- `Functional/Result.cs`
- `Pricing.ConfigureDiscount()`
- `Pricing.ConfigureSurcharge()`
- `Pricing.CurryPolicy()`
- `Pricing.ApplyPolicy()`
- `Pricing.BuildQuote()`
- `Pricing.QuoteBooking()`
- `Pricing.QuoteBatch()`

Functional programming concepts:

- Pure functions
- Higher-order functions
- Currying
- Partial application
- Function composition
- `Result<T>` and `Maybe<T>`
- `Map()` and `Bind()`

### Student 4 — Zhasmin

**LINQ Reports, Extension Methods, and Native C Integration**

Responsibilities:

- Generate revenue reports grouped by movie.
- Generate popularity reports grouped by movie.
- Implement reusable extension methods.
- Apply LINQ filtering, grouping, aggregation, and sorting.
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

## 2. Software Requirements

- .NET SDK 8 or later with .NET 8 targeting support.
- macOS with Xcode Command Line Tools or Linux with a C compiler.
- A native library compiled for the current operating system and architecture.

The application is a C# .NET 8 console project.

## 3. Build and Run on macOS

Open Terminal in the project root directory.

Restore dependencies:

```bash
dotnet restore
```

Build the C# application:

```bash
dotnet build
```

Compile the native C library:

```bash
sh Native/build-macos.sh
```

Run the application:

```bash
dotnet run
```

Run automated tests:

```bash
dotnet run -- --test
```

## 4. Build and Run on Linux

Open Terminal in the project root directory.

```bash
dotnet restore
dotnet build
sh Native/build-linux.sh
dotnet run
```

Run automated tests:

```bash
dotnet run -- --test
```

The native library must be rebuilt after extracting a clean project archive.

Do not enter terminal commands at the application's `Choose:` prompt. Exit the application first.

## 5. Project Structure

| File or Directory | Purpose |
|---|---|
| `Program.cs` | Console menu and application integration |
| `CinemaBookingFinal.csproj` | .NET project configuration |
| `Models/` | Immutable domain models |
| `Functional/` | Functional result and optional-value types |
| `Services/` | Booking, pricing, checkout, and query logic |
| `Persistence/` | JSON loading and saving |
| `Native/` | Native C code, P/Invoke adapter, and build scripts |
| `Data/` | Sample cinema data |
| `Tests/` | Automated test runner |
| `Reports/` | Project documentation and screenshots |
| `README.md` | Project documentation and setup instructions |

## 6. Main Application Features

The application provides 12 console menu commands.

Its main workflows include:

1. Browse available movies.
2. Search movies by title.
3. Filter movies by genre.
4. Display upcoming cinema sessions.
5. Check available seats.
6. Calculate ticket quotations.
7. Confirm ticket bookings.
8. Cancel existing bookings.
9. Generate revenue reports.
10. Generate popularity reports.
11. Add new cinema sessions.
12. Select configurable pricing policies.

The application uses immutable domain models and explicit result types to validate operations.

## 7. Pricing Rules

The system supports three customer categories:

| Customer Type | Discount |
|---|---|
| Adult | 0% |
| Student | 20% |
| Child | 40% |

Available pricing policies:

| Policy | Additional Discount |
|---|---|
| Standard | 0% |
| Promo | 10% |

The native C library calculates a booking fee of 500 basis points, equivalent to 5% of the discounted ticket price.

### Example

For an adult ticket with a base price of 2,500.00 KZT:

| Component | Standard | Promo |
|---|---|---|
| Base price | 2,500.00 KZT | 2,500.00 KZT |
| Policy discount | 0% | 10% |
| Discounted price | 2,500.00 KZT | 2,250.00 KZT |
| Native fee (5%) | 125.00 KZT | 112.50 KZT |
| Final price | 2,625.00 KZT | 2,362.50 KZT |

## 8. Functional Programming Techniques

The application demonstrates:

- Pure functions for pricing and queries.
- Immutable domain records and collections.
- Higher-order functions for configurable pricing.
- Currying and partial application.
- Functional composition using `Map()` and `Bind()`.
- Explicit error handling using `Result<T>`.
- Optional values using `Maybe<T>`.
- LINQ filtering, grouping, aggregation, and ordering.
- Lazy evaluation using `yield return`.
- Immutable state transitions for booking operations.
- Native C integration through P/Invoke.

## 9. JSON Persistence

The application uses JSON files to store cinema data.

The initial sample data is located in:

`Data/cinema.json`

The application loads data from a relative path and saves updated state after successful operations.

During execution, changes may be written to the output copy of the data file rather than directly modifying the original source file.

The persistence module validates loaded data and handles file-related errors.

## 10. Automated Testing

The application includes a deterministic automated test runner.

Run all tests using:

```bash
dotnet run -- --test
```

The observed local execution produced:

```text
TOTAL: 34 passed, 0 failed
```

The test suite covers:

- Pure functions and domain validation.
- Booking confirmation and cancellation.
- Seat availability.
- Customer discounts and pricing policies.
- Functional composition and failure propagation.
- LINQ reports and extension methods.
- JSON persistence and invalid file handling.
- Native C fee calculations.
- Rounding and integer boundary values.
- Complete booking and checkout workflows.

## 11. Limitations

The current implementation has several limitations:

- The application uses a console interface rather than a graphical interface.
- Data is stored locally in JSON files rather than a database.
- Authentication and separate user accounts are not implemented.
- Real payment processing is not supported.
- Online multi-user booking synchronization is not supported.
- The native C library must be compiled for the target operating system.

## 12. Submission Information

**Project:** Cinema Booking System

**Course:** Introduction to Functional Programming

**Academic Year:** 2026–2027

**Repository:** https://github.com/aisholpan777/CinemaBookingFinal

**Branch:** `main`

**Final Commit SHA:** `e719bd9d8362a7ec44d86517ad36a71fa3634c3e`

The shared project includes the C# source code, native C source, project configuration, tests, sample data, setup instructions, and group documentation.

Each student submits the complete project together with the shared Group Project Report and their own Individual Contribution Report.

## 13. References

- Microsoft Learn — C# Documentation: https://learn.microsoft.com/en-us/dotnet/csharp/
- Microsoft Learn — LINQ: https://learn.microsoft.com/en-us/dotnet/csharp/linq/
- Microsoft Learn — System.Text.Json: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/overview
- Microsoft Learn — Native Interoperability: https://learn.microsoft.com/en-us/dotnet/standard/native-interop/
- Introduction to Functional Programming — Course Materials and Final Project Requirements.
