
# Individual Contribution Report — Cinema Booking System

**Full name:** Aisholpan Bolatova  
**Student group:** IT-2512  
**Team members:** Aisholpan Bolatova, Dilnaz Momenzhanova, Zhuldyzay Galymzhankyzy, Zhasmin Askar  
**Project title:** Cinema Booking System  
**Course:** Introduction to Functional Programming  
**Academic year:** 2026–2027  
**Technology:** C# / .NET 8, LINQ, JSON, Native C

---

## 1. My Actual Contributions

My primary responsibility in the Cinema Booking System project is **Movies, Sessions, Console Interface, and System Integration**.

I developed the overall application architecture, implemented the console interface, integrated the functional modules, and verified the complete Cinema Booking System.

My work focuses on providing users with a clear interface for managing cinema operations, implementing movie and session functionality, and connecting the application's components into a working C#/.NET 8 console application.

The main files and methods related to my contribution are:

| Source File | Method or Component | Purpose |
|---|---|---|
| `Program.cs` | Main console interface | Displays menu options, processes user input, and presents results |
| `Models/Domain.cs` | `Movie`, `Session` | Defines immutable cinema domain records |
| `Services/Queries.cs` | `SearchMovies()` | Searches movies by title |
| `Services/Queries.cs` | `ByGenre()` | Filters movies by genre |
| `Services/Queries.cs` | `Upcoming()` | Retrieves upcoming cinema sessions |
| `Services/BookingService.cs` | `AddSession()` | Validates and adds new cinema sessions |

I developed and integrated the application's console interface, which provides **12 menu commands** for interacting with the system.

These commands support:

- Browsing available movies.
- Searching movies by title.
- Filtering movies by genre.
- Viewing upcoming cinema sessions.
- Checking available seats.
- Calculating ticket quotations.
- Confirming ticket bookings.
- Cancelling existing bookings.
- Viewing revenue and popularity reports.
- Adding new cinema sessions.
- Selecting pricing policies.

I implemented movie searching, genre filtering, upcoming session retrieval, and session management functionality.

I also connected the booking, pricing, persistence, reporting, and native fee modules through the main application workflow.

My contribution includes overall system integration, application verification, and automated testing.

The main objective of my work was to create a complete and reliable application in which the console interface communicates correctly with the functional business logic.

---

## 2. Functional Techniques

### 2.1 Pure Functions

The application separates business operations from console input and output.

For example, `SearchMovies()` and `ByGenre()` operate on movie data and return query results without modifying the original cinema state.

This improves predictability and testability.

Pure queries make it possible to verify movie searching and filtering independently from the console interface.

### 2.2 Immutable Data

The application uses immutable records and `ImmutableArray<T>` collections.

The `Movie` and `Session` records represent cinema information, while `CinemaState` contains the application's collections.

Immutable data helps prevent accidental modification of existing state.

Instead of changing the original state directly, operations return updated state values when necessary.

### 2.3 LINQ

LINQ is used to search, filter, and organize movie and session data.

For example, the application uses queries to find movies matching a title or genre.

This declarative approach makes collection operations easier to read and maintain.

LINQ also supports the separation of query logic from console interaction.

### 2.4 Lazy Evaluation

The `Upcoming()` method uses `yield return`.

This allows the application to produce upcoming sessions during enumeration instead of immediately creating a complete result collection.

Lazy evaluation delays the execution of iterator logic until the results are requested.

When a stable collection is needed, the console can materialize the results using `ToArray()`.

This demonstrates deferred execution and controlled evaluation of query results.

### 2.5 State Transitions

The `AddSession()` method validates session information and returns an updated `CinemaState` when the operation succeeds.

The original state is not modified in place.

For example, the method rejects attempts to add a session with an existing session ID.

This demonstrates safe state transitions and immutable application data.

### 2.6 Error Handling

The application uses `Result<T>` with `Ok` and `Fail` cases.

This makes successful and unsuccessful operations explicit and allows the console to display appropriate messages.

For example, an invalid session creation request returns a failure instead of modifying the existing cinema state.

---

## 3. Integration

My console interface connects the domain models, queries, booking services, pricing module, reporting functionality, native fee calculation, and JSON persistence.

### 3.1 Movie Search

The user selects option 2 and enters a movie title.

The console passes the keyword to `Queries.SearchMovies()`, receives the matching movies, and displays them.

For example, searching for `Interstellar` returns the corresponding movie.

This demonstrates the connection between console input and functional movie queries.

### 3.2 Genre Filtering

The user selects option 3 and enters a genre such as `Sci-Fi`.

The console uses `Queries.ByGenre()` to retrieve matching movies.

The matching results are displayed without modifying the original movie collection.

### 3.3 Upcoming Sessions

The user selects option 4.

The application calls `Queries.Upcoming()` and displays future cinema sessions.

This functionality demonstrates lazy evaluation using `yield return`.

The results can be materialized when the console needs a stable collection.

### 3.4 Adding a Session

The user selects option 11 and enters new session information.

The application calls `BookingService.AddSession()`.

If validation succeeds, the method returns an updated cinema state that can be saved through `JsonStore.Save()`.

If validation fails, the console displays an error.

For example, attempting to add a session with an existing ID is rejected.

### 3.5 End-to-End Integration

A complete booking workflow begins with selecting a movie and session, checking seat availability, calculating a ticket price, confirming the booking, and saving the updated state.

The workflow follows these steps:

1. The user selects a command from the console menu.
2. `Program.cs` processes the input.
3. The application calls the appropriate functional module.
4. Domain validation checks the requested operation.
5. The pricing and native fee modules calculate ticket costs when required.
6. The booking service processes the reservation.
7. The JSON persistence module saves successful state changes.
8. The console displays the final result.

The modules communicate through domain objects and explicit result types.

I integrated these components into a complete console application and verified that the workflows operate together correctly.

---

## 4. Tests I Wrote

I prepared and verified the automated testing of the application using `Tests/TestRunner.cs`.

The tests were executed using:

```bash
dotnet run -- --test
```

**Observed result:**

```text
TOTAL: 34 passed, 0 failed
```

The following tests are directly relevant to my primary responsibility.

### Test 1 — Lazy Processing and Snapshot

**Method:** `Queries.Upcoming()`

**Expected result:** The query returns two upcoming sessions, and the first session has ID 10.

**Actual result:** PASS.

This test verifies upcoming-session retrieval, lazy evaluation, and materialization using `ToArray()`.

### Test 2 — Add Session Validation

**Method:** `BookingService.AddSession()`

**Expected result:** Adding session ID 30 succeeds, while adding a session with an existing ID 10 fails.

**Actual result:** PASS.

This verifies successful session creation and duplicate-ID validation.

### Test 3 — Maybe Found and Missing

**Method:** `Find()`

**Expected result:**

- Searching for movie ID 1 returns `Maybe<Movie>.Some`.
- Searching for movie ID 99 returns `Maybe<Movie>.None`.

**Actual result:** PASS.

This demonstrates safe handling of existing and missing movie records.

### Overall Test Results

The complete test suite contains 34 automated tests covering:

- Domain validation.
- Movie and session queries.
- Pricing policies and discounts.
- Booking confirmation and cancellation.
- JSON persistence.
- LINQ queries and reports.
- Functional composition.
- Explicit error handling.
- Native C integration.

All 34 tests passed during the local execution.

The automated testing verified the behavior of the integrated application and confirmed that the functional modules work together correctly.

---

## 5. Defect or Technical Challenge

### Challenge: Maintaining Modular Architecture and Safe Session Management

One technical challenge was maintaining a clear separation between console interaction and business logic.

The application contains multiple workflows, and placing all validation and query operations directly inside `Program.cs` would make the code difficult to maintain and test.

I addressed this architectural challenge by organizing the application into separate modules.

- `Program.cs` handles console interaction and application coordination.
- `Queries.cs` contains movie and session queries.
- `BookingService.cs` performs validated state transitions.
- `Pricing.cs` handles configurable ticket calculations.
- `Checkout.cs` integrates pricing and booking operations.
- `JsonStore.cs` handles JSON persistence.
- The native C module calculates booking fees.

This modular structure makes the application easier to maintain, test, and extend.

Another important challenge was ensuring that invalid session data could not create an inconsistent application state.

The `AddSession()` operation rejects duplicate session IDs and returns an explicit failure result.

This behavior is verified by the automated test **Add session validation**, which passed.

These solutions improve the reliability of the application and support functional programming principles.

---

## 6. Limitations and Shared Work

The Cinema Booking System is a team project with four main responsibility areas.

| Team Member | Responsibility |
|---|---|
| Aisholpan | Movies, Sessions, Console Interface, and System Integration |
| Dilnaz | Booking Management, Domain Validation, and JSON Persistence |
| Zhuldyzay | Pricing Logic, Functional Composition, and Error Handling |
| Zhasmin | LINQ Reports, Extension Methods, and Native C Integration |

My main contribution includes the overall application architecture, movie and session functionality, console interface, system integration, and automated verification.

The application combines all four responsibility areas into one functional C#/.NET 8 system.

The movie and session modules provide information required by booking and pricing operations.

The booking module validates reservations and manages state transitions.

The pricing module calculates ticket costs, while the native C component provides booking fee calculations.

The reporting module generates cinema statistics, and JSON persistence stores application data.

I integrated these components through the console interface and verified the complete application.

### Current Limitations

The application has several limitations:

- It uses a console interface instead of a graphical interface.
- Data is stored in local JSON files.
- Authentication and separate user accounts are not implemented.
- Multi-user online booking synchronization is not supported.
- The native C library must be compiled for the operating system being used.

These limitations do not prevent the application from demonstrating its main functional programming requirements.

---

## 7. Defense Preparation

### 7.1 Normal Flow — Movie Search

During the defense, I can demonstrate the movie search functionality by running the application and selecting option 2.

After entering `Interstellar`, the application displays the matching movie.

I will explain how `Program.cs` passes the input to `Queries.SearchMovies()` and displays the returned results.

This demonstrates functional querying and separation between the console interface and business logic.

### 7.2 Failure Flow — Duplicate Session

I can demonstrate validation by attempting to add a session with an existing ID.

`BookingService.AddSession()` returns `Result<CinemaState>.Fail`.

The operation does not update the existing cinema state.

This demonstrates safe state transitions and explicit error handling.

### 7.3 Possible Live Code Change

A possible live coding task is extending movie filtering.

For example, the instructor may ask me to modify the movie query to filter by both genre and minimum duration.

I would update the relevant method in `Services/Queries.cs`, rebuild the application, and run the tests.

After making the change, I would execute:

```bash
dotnet build
dotnet run -- --test
dotnet run
```

This would demonstrate how to extend a functional query while keeping the rest of the application unchanged.

### 7.4 Functional Programming Concepts

**Pure function:** A function that produces a result without modifying external state.

**Immutable data:** Data that cannot be modified directly after creation.

**LINQ:** A C# feature used to query, filter, and transform collections.

**Lazy evaluation:** Delaying computation until results are requested.

**`yield return`:** A C# statement used to produce elements during iteration.

**State transition:** A domain operation that returns a new state instead of modifying the original state.

**Result type:** A type that explicitly represents success or failure.

**Separation of concerns:** Organizing application functionality into modules with distinct responsibilities.

---

## 8. Conclusion

I developed and integrated the Cinema Booking System, with my primary responsibility focused on movies, sessions, console interaction, and overall system integration.

My contribution includes the application architecture, immutable movie and session models, functional queries, session management, console workflows, and automated verification.

The project demonstrates functional programming concepts such as pure queries, immutable data, LINQ, lazy evaluation, safe state transitions, and explicit error handling.

The application was successfully built and tested, with all 34 automated tests passing.

Through this project, I gained practical experience in organizing a C# application into functional modules, integrating different workflows, and validating application behavior through automated testing.
