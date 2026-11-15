
# Individual Contribution Report — Cinema Booking System

**Full name:** Dilnaz Momenzhanova  
**Student group:** IT-2512  
**Team members:** Aisholpan Bolatova, Dilnaz Momenzhanova, Zhuldyzay Galymzhankyzy, Zhasmin Askar  
**Project title:** Cinema Booking System  
**Course:** Introduction to Functional Programming  
**Academic year:** 2026–2027  
**Technology:** C# / .NET 8, LINQ, JSON, Native C

---

## 1. My Actual Contributions

My responsibility in the Cinema Booking System project is **Booking Management, Domain Validation, and JSON Persistence**.

My work focuses on managing the reservation lifecycle, preventing invalid seat bookings, validating domain inputs, and storing cinema data in JSON format.

The main files and methods related to my contribution are:

| Source File | Method or Component | Purpose |
|---|---|---|
| `Models/Domain.cs` | `ValidatedSelection.Create()` | Validates booking input before further processing |
| `Services/BookingService.cs` | `Confirm()` | Confirms a valid ticket reservation |
| `Services/BookingService.cs` | `Cancel()` | Cancels an existing booking |
| `Services/Queries.cs` | `AvailableSeats()` | Determines which seats remain available |
| `Services/Queries.cs` | `AvailableSeatsChecked()` | Performs checked seat availability lookup |
| `Persistence/JsonStore.cs` | `Load()` | Loads and validates cinema data |
| `Persistence/JsonStore.cs` | `Save()` | Saves the updated cinema state |

I developed booking confirmation and cancellation operations, validated booking inputs, implemented seat availability queries, and integrated JSON persistence into the application.

The booking functionality checks whether a requested session exists, whether the selected seat is valid, and whether it is already occupied.

The system also prevents repeated cancellation and rejects invalid booking information.

JSON persistence allows the application to preserve valid changes between executions.

My contribution also includes testing booking validation, state transitions, duplicate booking prevention, seat availability, and JSON file operations.

These components ensure that reservations are processed safely and that application data remains consistent.

---

## 2. Functional Techniques

### 2.1 Immutable State

The application represents its data using immutable domain records and `ImmutableArray<T>` collections.

When a booking is confirmed or cancelled, the service returns an updated `CinemaState` instead of modifying the original state directly.

This makes state transitions predictable and easier to test.

### 2.2 Validated Input

The method `ValidatedSelection.Create()` checks booking information before it is used in further operations.

It validates conditions such as:

- The requested session must exist.
- The seat number must be within the session's capacity.
- The customer type must be supported.
- Missing input must be rejected.

Successful validation produces `Result<ValidatedSelection>.Ok`.

Invalid input produces `Result<ValidatedSelection>.Fail`.

### 2.3 Safe State Transitions

The method `BookingService.Confirm()` creates a booking only when the required business rules are satisfied.

For example, a seat that has already been reserved cannot be booked again.

The method `BookingService.Cancel()` changes a confirmed booking to a cancelled state.

Attempting to cancel the same booking again is rejected.

These operations demonstrate explicit business rules and controlled state transitions.

### 2.4 Functional Error Handling

The application uses the `Result<T>` type to represent success and failure.

This makes expected errors part of the function's return value.

For example, attempting to reserve an occupied seat returns a failure instead of creating an invalid booking.

### 2.5 LINQ and Collection Queries

The `AvailableSeats()` method uses booking information to determine which seats are not occupied.

Confirmed bookings reserve seats, while cancelled bookings do not occupy them.

This functionality supports the booking process by allowing customers to select available seats.

### 2.6 Separation of Pure Logic and Side Effects

Booking validation and state transitions are separated from file operations.

The booking service processes domain data, while `JsonStore` handles reading and writing JSON files.

This separation allows booking rules to be tested without accessing the file system.

---

## 3. Integration

The booking module connects movie sessions, pricing, seat availability, and persistence.

### 3.1 Booking Confirmation Workflow

The booking process follows these steps:

1. The user selects option 7 — Book ticket.
2. The application receives the session ID, seat number, customer type, and pricing policy.
3. The input is validated.
4. The pricing module calculates the ticket quote.
5. The booking service checks the reservation conditions.
6. A successful operation produces an updated cinema state.
7. The persistence module saves the updated state.

This workflow prevents invalid reservations and preserves successful bookings.

### 3.2 Seat Availability Workflow

The user selects option 5 — Available seats.

The application retrieves the relevant session and its confirmed bookings.

It then determines which seat numbers remain available.

For example, if a session has two seats and seat 1 is already reserved, the available-seat query returns seat 2.

### 3.3 Booking Cancellation Workflow

The user selects option 8 — Cancel booking.

The system searches for the booking and checks whether it can be cancelled.

If the booking is confirmed, the service returns an updated state with the booking marked as cancelled.

If the booking is already cancelled, the operation returns a failure.

### 3.4 JSON Persistence

The `JsonStore.Load()` method reads cinema data from a JSON file.

It validates the loaded data and returns an explicit result.

The `JsonStore.Save()` method writes a valid cinema state to a JSON file.

File operations are kept outside the pure domain logic.

### 3.5 Integration with Other Modules

The booking module depends on session information from the domain models.

It uses pricing results produced by the pricing module.

The console interface displays success or failure messages.

The reporting module uses confirmed bookings to calculate revenue and popularity.

This creates a complete workflow connecting different functional components.

---

## 4. Tests Relevant to My Responsibility

The project includes an automated test runner in `Tests/TestRunner.cs`.

The test suite was executed using:

```bash
dotnet run -- --test
```

**Observed result:**

```text
TOTAL: 34 passed, 0 failed
```

The following tests verify booking, validation, and persistence functionality.

### Test 1 — Factory Accepts Valid Input

**Method:** `ValidatedSelection.Create()`

**Expected result:** Valid booking input returns `Result<ValidatedSelection>.Ok`.

**Actual result:** PASS.

### Test 2 — Factory Rejects Missing Session

**Method:** `ValidatedSelection.Create()`

**Expected result:** A booking request for session ID 99, which does not exist, returns `Fail`.

**Actual result:** PASS.

### Test 3 — Factory Rejects Seat Boundary

**Method:** `ValidatedSelection.Create()`

**Expected result:** Requesting seat 3 in a session with only two seats returns `Fail`.

**Actual result:** PASS.

### Test 4 — Factory Rejects Customer Type

**Method:** `ValidatedSelection.Create()`

**Expected result:** An unsupported customer type returns `Fail`.

**Actual result:** PASS.

### Test 5 — Factory Rejects Missing Input

**Method:** `ValidatedSelection.Create()`

**Expected result:** Null booking input returns `Fail`.

**Actual result:** PASS.

### Test 6 — Booking and Duplicate Rejected

**Method:** `BookingService.Confirm()`

**Expected result:** The first valid booking succeeds, while attempting to book the same seat again fails. The original state remains unchanged.

**Actual result:** PASS.

### Test 7 — Cancellation and Repeat

**Method:** `BookingService.Cancel()`

**Expected result:** The first cancellation succeeds, while a repeated cancellation fails.

**Actual result:** PASS.

### Test 8 — Available Seats After Booking

**Method:** `Queries.AvailableSeats()`

**Expected result:** When seat 1 is reserved in a two-seat session, the query returns only seat 2.

**Actual result:** PASS.

### Test 9 — Missing File

**Method:** `JsonStore.Load()`

**Expected result:** Loading a nonexistent file returns `Result<CinemaState>.Fail`.

**Actual result:** PASS.

### Test 10 — Malformed File

**Method:** `JsonStore.Load()`

**Expected result:** Loading malformed JSON returns `Fail`.

**Actual result:** PASS.

### Test 11 — JSON Round Trip

**Methods:** `JsonStore.Save()` and `JsonStore.Load()`

**Expected result:** Saving a valid cinema state succeeds, and loading the saved file also succeeds.

**Actual result:** PASS.

### Test 12 — Invalid JSON Data Rejected

**Method:** `JsonStore.Load()`

**Expected result:** JSON containing a booking that references a nonexistent session is rejected.

**Actual result:** PASS.

### Additional Booking Validation Tests

The complete suite also includes:

- Reject negative quote.
- Reject inconsistent quote total.
- Reject booking after session start.
- Valid quote remains bookable.

All four tests passed.

### Testing Summary

| Test Category | Result |
|---|---|
| Booking input validation | PASS |
| Booking confirmation | PASS |
| Duplicate booking prevention | PASS |
| Booking cancellation | PASS |
| Seat availability | PASS |
| Missing and malformed files | PASS |
| JSON persistence | PASS |
| Invalid persisted data | PASS |

The complete application passed all 34 automated tests.

The tests cover booking input validation, safe state transitions, duplicate booking prevention, cancellation, seat availability, JSON persistence, and invalid persisted data.

All tests related to my responsibility passed successfully.

---

## 5. Defect or Technical Challenge

### Challenge: Preventing Invalid Booking States

An important technical challenge in the booking module is preventing inconsistent reservations.

For example, allowing two customers to reserve the same seat would create conflicting bookings.

The implementation addresses this by validating the requested seat against existing confirmed bookings before accepting a reservation.

The booking operation returns `Result<CinemaState>.Fail` when the seat is already occupied.

Another important challenge is handling invalid JSON data.

A file may contain malformed JSON or records that reference nonexistent sessions.

The persistence layer validates loaded data and returns a failure when the data is invalid.

These safeguards are verified by the automated tests **Booking and duplicate rejected** and **Invalid JSON data rejected**.

The tests passed during the observed execution.

These validation mechanisms help maintain consistent booking information and prevent corrupted application state.

---

## 6. Limitations and Shared Work

The Cinema Booking System is a team project with four main responsibility areas.

| Team Member | Responsibility |
|---|---|
| Aisholpan | Movies, Sessions, Console Interface, and System Integration |
| Dilnaz | Booking Management, Domain Validation, and JSON Persistence |
| Zhuldyzay | Pricing Logic, Functional Composition, and Error Handling |
| Zhasmin | LINQ Reports, Extension Methods, and Native C Integration |

My responsibility focuses on booking workflows, validation, seat availability, and persistence.

My modules are connected with the other team members' components to create a complete Cinema Booking System.

The booking module depends on shared domain models, pricing calculations, and console interaction.

The persistence module saves valid application states and supports reliable data loading.

This integration allows customers to book and cancel tickets while maintaining consistent reservation information.

### Current Limitations

The application has several limitations:

- It uses a console interface instead of a graphical interface.
- Data is stored locally in JSON files.
- Authentication and separate user accounts are not implemented.
- Online multi-user booking synchronization is not supported.
- The application does not integrate with a real payment provider.

Despite these limitations, the system demonstrates safe booking operations, immutable state transitions, explicit error handling, and local persistence.

---

## 7. Defense Preparation

### 7.1 Normal Flow — Successful Booking

During the defense, I can demonstrate a successful ticket reservation.

I select option 7 from the console menu.

I enter a valid session ID, an available seat number, and a supported customer type.

The system calculates the ticket price and attempts to confirm the reservation.

If all validation checks succeed, the booking is confirmed and the updated state is saved.

I can explain how `ValidatedSelection.Create()`, the pricing module, `BookingService.Confirm()`, and `JsonStore.Save()` work together.

### 7.2 Failure Flow — Duplicate Booking

I can demonstrate a failure by attempting to reserve a seat that is already booked.

The system detects that the seat is occupied and returns a failure.

The original state remains valid.

This demonstrates safe state transitions and explicit error handling.

### 7.3 Possible Live Code Change

A possible live coding task is adding another validation rule to booking confirmation.

For example, the instructor may ask the team to reject a booking when the requested seat number is invalid.

I would identify the validation logic in `ValidatedSelection.Create()` or the booking service, apply the requested change, and add an automated test.

After making the change, I would run:

```bash
dotnet build
dotnet run -- --test
```

This would verify that the updated validation works without breaking existing functionality.

### 7.4 Functional Programming Concepts

**Immutability:** Existing domain state is not modified directly; operations produce updated values.

**Result type:** Success and failure are represented explicitly.

**State transition:** A domain operation changes the logical state by returning a new state value.

**Validation:** Business rules are checked before an operation is accepted.

**Separation of concerns:** Booking logic and JSON file operations are implemented in different modules.

---

## 8. Conclusion

My contribution to the Cinema Booking System focuses on booking operations, state validation, seat availability, and JSON persistence.

I developed booking confirmation and cancellation functionality, validated booking inputs, implemented seat availability queries, and integrated JSON persistence.

This part of the project demonstrates important functional programming principles, including immutable data, explicit error handling, safe state transitions, and separation of domain logic from file operations.

The automated tests verify successful bookings, invalid input rejection, duplicate booking prevention, cancellation, seat availability, and JSON persistence.

The complete application passed all 34 automated tests during the observed local execution.

The project provides a practical example of implementing reliable booking workflows using C# and functional programming techniques.
