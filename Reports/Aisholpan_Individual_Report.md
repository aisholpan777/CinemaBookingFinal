# Individual Contribution Report — Cinema Booking System

**Full name:** Aisholpan Bolatova  
**Student group:** It-2512 
**Team members:** Aisholpan Bolatova, Dilnaz, Zhuldyzay, Zhasmin  
**Project title:** Cinema Booking System  
**Course:** Introduction to Functional Programming  
**Technology:** C# / .NET 8, LINQ, JSON, Native C

---

## 1. My Actual Contributions

I developed and integrated the Cinema Booking System as a C# console application. For our team presentation, my primary responsibility is **Movies, Sessions, and Console Interface**.

My work includes the application structure, console workflows, integration of the functional modules, and verification of the complete project.

The main files and methods related to my assigned section are:

| File | Method or component | Functionality |
|---|---|---|
| `Program.cs` | Main console interface | Displays menu options, reads user input, and shows results |
| `Models/Domain.cs` | `Movie`, `Session` | Defines immutable cinema domain records |
| `Services/Queries.cs` | `SearchMovies()` | Searches movies by title |
| `Services/Queries.cs` | `ByGenre()` | Filters movies by genre |
| `Services/Queries.cs` | `Upcoming()` | Retrieves upcoming cinema sessions |
| `Services/BookingService.cs` | `AddSession()` | Validates and adds new sessions |

The console interface provides 12 menu commands, including browsing movies, searching, genre filtering, viewing upcoming sessions, checking seats, ticket quotation, booking, cancellation, reports, session creation, and pricing policies.

My main objective was to create a clear connection between user interaction and the application's functional business logic.

## 2. Functional Techniques

### Pure Functions

The application separates business operations from console input and output.

For example, `SearchMovies()` and `ByGenre()` operate on movie data and return query results without modifying the original cinema state.

This improves predictability and testability.

### Immutable Data

The application uses immutable records and `ImmutableArray<T>` collections.

The `Movie` and `Session` records represent cinema information, while `CinemaState` contains the application's collections.

Immutable data helps prevent accidental modification of existing state.

### LINQ

LINQ is used to search, filter, and organize movie and session data.

This declarative approach makes collection operations easier to read and maintain.

### Lazy Evaluation

The `Upcoming()` method uses `yield return`.

This allows the application to produce upcoming sessions during enumeration instead of immediately creating a complete result collection.

### State Transitions

The `AddSession()` method validates session information and returns an updated `CinemaState` when the operation succeeds.

The original state is not modified in place.

### Error Handling

The application uses `Result<T>` with `Ok` and `Fail` cases.

This makes successful and unsuccessful operations explicit and allows the console to display appropriate messages.

## 3. Integration

My console interface connects the domain models, queries, booking services, pricing module, and JSON persistence.

### Movie Search

The user selects option `2` and enters a movie title.

The console passes the keyword to `Queries.SearchMovies()`, receives the matching movies, and displays them.

For example, searching for `Interstellar` returns the corresponding movie.

### Genre Filtering

The user selects option `3` and enters a genre such as `Sci-Fi`.

The console uses `Queries.ByGenre()` to retrieve matching movies.

### Upcoming Sessions

The user selects option `4`.

The application calls `Queries.Upcoming()` and displays future cinema sessions.

### Adding a Session

The user selects option `11` and enters new session information.

The application calls `BookingService.AddSession()`.

If validation succeeds, the updated state can be saved through `JsonStore.Save()`.

If validation fails, the console displays an error.

### End-to-End Integration

A complete booking workflow begins with selecting a movie and session, checking seat availability, calculating a ticket price, confirming the booking, and saving the updated state.

The modules communicate through domain objects and explicit result types.

## 4. Tests I Wrote

I prepared and verified the automated testing of the application using `Tests/TestRunner.cs`.

The tests were executed using:

```bash
dotnet run -- --test
```

**Actual result:**

```text
TOTAL: 34 passed, 0 failed
```

The following tests are directly relevant to my assigned responsibility.

### Test 1: Lazy processing and snapshot

**Method:** `Queries.Upcoming()`

**Expected result:** The query returns two upcoming sessions, and the first session has ID `10`.

**Actual result:** PASS.

This test verifies upcoming-session retrieval and materialization using `ToArray()`.

### Test 2: Add session validation

**Method:** `BookingService.AddSession()`

**Expected result:** Adding session ID `30` succeeds, while adding a session with an existing ID `10` fails.

**Actual result:** PASS.

This verifies successful session creation and duplicate-ID validation.

### Test 3: Maybe found and missing

**Method:** `Find()`

**Expected result:** Searching for movie ID `1` returns `Maybe<Movie>.Some`, while searching for movie ID `99` returns `Maybe<Movie>.None`.

**Actual result:** PASS.

This demonstrates safe handling of existing and missing movie records.

### Overall Test Results

The complete test suite contains 34 automated tests covering domain validation, pricing, booking, cancellation, JSON persistence, LINQ queries, functional composition, and native C integration.

All 34 tests passed during the local execution.

## 5. Defect or Technical Challenge

One technical challenge was maintaining a clear separation between console interaction and business logic.

The application contains multiple workflows, and placing all validation and query operations directly inside `Program.cs` would make the code difficult to maintain and test.

I addressed this architectural challenge by organizing the application into separate modules.

`Program.cs` handles user interaction, `Queries.cs` contains movie and session queries, `BookingService.cs` performs validated state transitions, and `JsonStore.cs` handles persistence.

Another important challenge was ensuring that invalid session data could not create an inconsistent application state.

The `AddSession()` operation rejects duplicate session IDs and returns an explicit failure result.

This behavior is verified by the automated test `Add session validation`, which passed.

## 6. Limitations and Shared Work

The Cinema Booking System is a team project with four assigned responsibility areas.

| Team member | Assigned responsibility |
|---|---|
| Aisholpan | Movies, Sessions, and Console Interface |
| Dilnaz | Booking, Validation, and JSON Persistence |
| Zhuldyzay | Pricing, Functional Composition, and Error Handling |
| Zhasmin | LINQ Reports, Extension Methods, and Native C Integration |

I prepared the integrated project implementation, while these areas define how the team will present and explain the system.

The application has several limitations:

- It uses a console interface instead of a graphical interface.
- Data is stored in local JSON files.
- Authentication and separate user accounts are not implemented.
- Multi-user online booking synchronization is not supported.
- The native C library must be compiled for the operating system being used.

These limitations do not prevent the application from demonstrating its main functional programming requirements.

## 7. Defense Preparation

### Normal Flow: Movie Search

I can demonstrate the movie search functionality by running the application and selecting option `2`.

After entering `Interstellar`, the application displays the matching movie.

I will explain how `Program.cs` passes the input to `Queries.SearchMovies()` and displays the returned results.

### Failure Flow: Duplicate Session

I can demonstrate validation by attempting to add a session with an existing ID.

`BookingService.AddSession()` returns `Result<CinemaState>.Fail`.

The operation does not update the existing cinema state.

### Possible Live Code Change

A possible live coding task is to extend movie filtering.

For example, I could modify the movie query to filter by both genre and minimum duration.

I would update the relevant method in `Services/Queries.cs`, rebuild the application, and run the tests.

Commands:

```bash
dotnet build
dotnet run -- --test
dotnet run
```

This would demonstrate how to extend a functional query while keeping the rest of the application unchanged.

## Conclusion

I developed and integrated the Cinema Booking System, with my primary presentation responsibility focused on movies, sessions, and console interaction.

The project demonstrates functional programming concepts such as pure queries, immutable data, LINQ, lazy evaluation, safe state transitions, and explicit error handling.

The application was successfully built and tested, with all 34 automated tests passing.

Through this project, I gained practical experience in organizing a C# application into functional modules, integrating different workflows, and validating application behavior through automated testing.