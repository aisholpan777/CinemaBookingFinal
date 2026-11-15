# Individual Contribution Report — Cinema Booking System

**Full name:** Zhasmin Askar 
**Student group:** IT-2512 
**Team members:** Aisholpan, Dilnaz, Zhuldyzay, Zhasmin  
**Project title:** Cinema Booking System  
**Course:** Introduction to Functional Programming  
**Academic year:** 2026–2027  
**Technology:** C# / .NET 8, LINQ, JSON, Native C

---

## 1. My Actual Contributions

My assigned responsibility in the Cinema Booking System project is **LINQ Reports, Extension Methods, and Native C Integration**.

This area focuses on generating cinema statistics, processing booking collections using functional queries, and integrating native C code with the C# application.

The main components associated with my responsibility are:

| Source file | Method or component | Purpose |
|---|---|---|
| `Services/Queries.cs` | `Revenue()` | Generates revenue statistics for movies |
| `Services/Queries.cs` | `Popularity()` | Generates movie popularity statistics |
| `Services/Queries.cs` | `Active()` | Filters confirmed bookings |
| `Services/Queries.cs` | `ForSession()` | Filters bookings for a selected session |
| `Native/cinemafee.c` | `cinema_fee()` | Calculates the native booking fee |
| `Native/NativeFee.cs` | `NativeFeeCalculator` | Connects C# with the native C library |
| `Native/NativeFee.cs` | `FailingFeeCalculator` | Simulates fee calculation failures for testing |
| `Native/build-macos.sh` | Build script | Compiles the native library on macOS |
| `Native/build-linux.sh` | Build script | Compiles the native library on Linux |

The reporting functionality provides two main reports: revenue and movie popularity.

The native C component calculates the booking fee using integer arithmetic and is integrated into the checkout workflow.

**Contribution clarification:** These components are assigned to me for the team defense. The integrated implementation was prepared centrally, so this responsibility assignment does not independently establish personal authorship of every method.

---

## 2. Functional Techniques

### 2.1 LINQ Queries

The project uses Language Integrated Query (LINQ) to process cinema data.

LINQ allows the application to filter, group, aggregate, and order collections declaratively.

The reporting module uses these operations to produce meaningful statistics from confirmed bookings.

### 2.2 Revenue Report

The method `Queries.Revenue()` generates revenue statistics for movies.

It processes booking data and calculates the revenue associated with each movie.

The report demonstrates LINQ operations such as filtering, grouping, projection, aggregation, and ordering.

Only relevant confirmed bookings contribute to the report.

The result is represented using revenue report records.

### 2.3 Popularity Report

The method `Queries.Popularity()` calculates movie popularity based on confirmed ticket bookings.

It groups booking information by movie and counts relevant reservations.

This allows the application to identify movies with higher booking activity.

The report also uses deterministic ordering to handle equal results.

### 2.4 Extension Methods

The project includes two custom extension methods:

**`Active()`**

This method filters booking collections to include confirmed bookings.

**`ForSession()`**

This method filters bookings by a selected session ID.

Extension methods allow reusable query logic to be called using method syntax.

For example:

`bookings.Active().ForSession(10)`

This combines two filtering operations in a readable way.

### 2.5 Pure Functional Queries

The reporting methods calculate results from existing cinema data without modifying the original state.

This makes the reports predictable and suitable for automated testing.

### 2.6 Native C Integration

The project integrates a C function into the C# application using Platform Invocation Services (P/Invoke).

The native function is:

`cinema_fee()`

It receives a ticket price in cents and a fee rate in basis points.

The function calculates the booking fee using integer arithmetic.

For example, a rate of `500` basis points represents 5%.

The native library is compiled separately and loaded by the .NET application.

### 2.7 Error Handling

The `NativeFeeCalculator` returns a `Result<long>` representing success or failure.

Invalid input, unavailable native libraries, and other supported native integration errors are handled explicitly.

The checkout workflow can reject the operation if the fee calculation fails.

---

## 3. Integration

The reporting and native integration modules connect with the cinema booking workflow.

### 3.1 Revenue Report Workflow

The user selects option `9` — Revenue report.

The application retrieves booking information from the current cinema state.

The report processes confirmed bookings and calculates revenue grouped by movie.

The console displays the resulting statistics.

**Observed example:**

```text
Choose: 9
Interstellar: 1 tickets, 2100,00 KZT
```

This demonstrates the revenue report using actual application data.

### 3.2 Popularity Report Workflow

The user selects option `10` — Popularity report.

The application groups confirmed bookings by movie and counts the number of tickets.

The results are displayed as a popularity ranking.

This helps cinema staff understand which movies have more confirmed reservations.

### 3.3 Extension Method Integration

The reporting module uses reusable query operations.

For example, `Active()` selects confirmed bookings, while `ForSession()` restricts the results to a particular session.

These methods improve readability and reduce repeated filtering logic.

### 3.4 Native Fee Workflow

The native C function is integrated into ticket quotation and checkout.

The process follows these steps:

1. The application calculates the discounted ticket price.
2. The C# native fee adapter receives the price and fee rate.
3. P/Invoke calls the compiled C function.
4. The native function calculates the fee.
5. The result is returned to C#.
6. The checkout module combines the fee with the ticket price.

If native fee calculation fails, the checkout workflow returns a failure instead of confirming a booking.

### 3.5 Native Fee Example

Consider a ticket price of 10,000 cents.

The booking fee rate is 500 basis points, equivalent to 5%.

| Component | Amount |
|---|---:|
| Ticket price | 10,000 cents |
| Booking fee | 500 cents |
| Final total | 10,500 cents |

The native fee calculation is also tested with rounding and boundary values.

### 3.6 Integration with Other Modules

The reporting module depends on movie, session, and booking domain records.

The booking service produces confirmed and cancelled reservations.

The reporting methods use this information to calculate statistics.

The pricing and checkout modules call the native fee calculator.

The console interface displays the generated reports and final ticket quotations.

---

## 4. Tests Relevant to My Responsibility

The application includes an automated test runner in `Tests/TestRunner.cs`.

The tests were executed using:

`dotnet run -- --test`

**Observed result:**

`TOTAL: 34 passed, 0 failed`

The following tests verify reporting, extension methods, and native integration.

### Test 1 — Empty reports

**Methods:** `Queries.Revenue()`, `Queries.Popularity()`

**Expected result:** Both reports return empty results when there are no bookings.

**Actual result:** PASS.

### Test 2 — Extensions

**Methods:** `Active()`, `ForSession()`

**Expected result:** Filtering confirmed bookings for session `10` returns one booking, while filtering for session `20` returns none.

**Actual result:** PASS.

### Test 3 — Revenue and popularity ties

**Methods:** `Queries.Revenue()`, `Queries.Popularity()`

**Expected result:** When two movies have equal report values, the results are ordered deterministically as `A`, then `B`.

**Actual result:** PASS.

### Test 4 — Native real call

**Method:** `NativeFeeCalculator.Calculate()`

**Expected result:** A ticket price of 10,000 cents with a fee rate of 500 basis points produces a fee of 500 cents.

**Actual result:** PASS.

### Test 5 — Native failure adapter

**Method:** `FailingFeeCalculator.Calculate()`

**Expected result:** The failing adapter returns `Result<long>.Fail`.

**Actual result:** PASS.

### Test 6 — Native checkout workflow

**Method:** `Checkout.BookWithNativeFee()`

**Expected result:** A booking with a ticket price of 10,000 cents and a native fee of 500 cents is confirmed with a total of 10,500 cents.

**Actual result:** PASS.

### Test 7 — Native failure preserves state

**Method:** `Checkout.BookWithNativeFee()`

**Expected result:** A native fee calculation failure prevents booking confirmation and preserves the original state.

**Actual result:** PASS.

### Test 8 — Native fee rounds up

**Method:** `NativeFeeCalculator.Calculate()`

**Expected result:** A ticket price of 1,001 cents with a 5% fee produces 51 cents after rounding up.

**Actual result:** PASS.

### Test 9 — Native fee handles maximum value

**Method:** `NativeFeeCalculator.Calculate()`

**Expected result:** Calculating a 100% fee on `long.MaxValue` returns `long.MaxValue` without arithmetic overflow.

**Actual result:** PASS.

### Test 10 — Native fee rejects invalid rate

**Method:** `NativeFeeCalculator.Calculate()`

**Expected result:** Fee rates below 0 or above 10,000 basis points return failure results.

**Actual result:** PASS.

### Testing Summary

| Test category | Result |
|---|---|
| Empty LINQ reports | PASS |
| Custom extension methods | PASS |
| Deterministic report ordering | PASS |
| Native C fee calculation | PASS |
| Native failure handling | PASS |
| Native checkout integration | PASS |
| Native rounding | PASS |
| Maximum integer boundary | PASS |
| Invalid fee rates | PASS |

All 34 automated tests passed during the observed local execution.

These tests verify functionality relevant to my assigned area; their individual authorship is not established by the test runner.

---

## 5. Defect or Technical Challenge

### Challenge: Native C Library Integration and Safe Integer Arithmetic

An important technical challenge in this responsibility area is integrating native C code with a managed C# application.

The C function must be compiled into a shared library compatible with the operating system and processor architecture.

The application uses P/Invoke to call the native function.

On macOS, the native library is compiled as a `.dylib` file.

On Linux, it is compiled as a `.so` file.

Another important challenge is handling large integer values safely during fee calculation.

The native function uses integer arithmetic and validates its inputs.

The implementation is tested with `long.MaxValue`, invalid fee rates, and values requiring rounding.

The automated tests `Native fee handles maximum value`, `Native fee rounds up`, and `Native fee rejects invalid rate` all passed.

These tests demonstrate that the implemented fee calculation handles the tested boundary cases.

This section describes verified implementation challenges rather than an individually documented debugging incident.

---

## 6. Limitations and Shared Work

The Cinema Booking System has four assigned responsibility areas.

| Team member | Responsibility |
|---|---|
| Aisholpan | Movies, Sessions, and Console Interface |
| Dilnaz | Booking, State Validation, and JSON Persistence |
| Zhuldyzay | Pricing, Functional Composition, and Error Handling |
| Zhasmin | LINQ Reports, Extension Methods, and Native C Integration |

My assigned responsibility focuses on LINQ reporting, reusable extension methods, and native C integration.

The reporting module depends on shared domain models and booking data.

The native fee module is used by the pricing and checkout workflows.

The integrated project implementation was prepared centrally and divided into responsibility areas for the team defense.

### Current Limitations

The application has several limitations:

- Reports are displayed through the console rather than a graphical dashboard.
- Data is stored locally in JSON files.
- The native C library must be compiled for the target operating system.
- Online multi-user synchronization is not implemented.
- The application does not provide a real payment integration.

Despite these limitations, the system demonstrates LINQ aggregation, extension methods, functional queries, native interoperability, and explicit error handling.

---

## 7. Defense Preparation

### 7.1 Normal Flow — Revenue Report

During the defense, I can demonstrate the revenue report.

I start the application and select option `9`.

The application processes confirmed bookings and displays revenue statistics grouped by movie.

I can explain how LINQ filtering, grouping, aggregation, and ordering are used to generate the results.

### 7.2 Normal Flow — Native Fee Calculation

I can demonstrate the native booking fee using the automated test `Native real call`.

The test calls `NativeFeeCalculator.Calculate(10000, 500)`.

The expected result is 500 cents.

I can explain how P/Invoke allows C# to call a function compiled from C code.

### 7.3 Failure Flow — Invalid Native Rate

I can demonstrate a failure using an invalid fee rate.

For example, a rate of `10001` basis points is outside the supported range.

The native fee calculator returns `Result<long>.Fail`.

This prevents the application from accepting an invalid fee calculation.

### 7.4 Possible Live Code Change

A possible live coding task is modifying the revenue report to filter results by a minimum revenue amount.

I would update the relevant query in `Services/Queries.cs` and add an automated test verifying the expected result.

Another possible task is adding a new native fee boundary test.

After making the change, I would execute:

```bash
dotnet build
dotnet run -- --test
```

This verifies that the modified logic works without breaking existing functionality.

### 7.5 Functional Programming Concepts

**LINQ:** A C# feature used to query, transform, and aggregate collections.

**GroupBy:** Groups collection elements according to a selected key.

**Aggregation:** Combines multiple values into a summary, such as total revenue or ticket count.

**Extension method:** A static method that can be called using instance-method syntax.

**Pure function:** A function that produces results without modifying external state.

**P/Invoke:** A .NET interoperability mechanism used to call native functions.

**Basis points:** A unit representing one-hundredth of a percentage point.

**Result type:** A type that explicitly represents successful and unsuccessful outcomes.

---

## 8. Conclusion

My assigned responsibility in the Cinema Booking System focuses on LINQ reports, extension methods, and native C integration.

This part of the project demonstrates functional data processing, reusable collection queries, deterministic reporting, and interoperability between C# and C.

The revenue and popularity reports provide useful statistics based on confirmed bookings.

The native C module calculates booking fees and supports explicit failure handling.

Automated tests verify reporting behavior, extension methods, native fee calculations, rounding, boundary values, and checkout integration.

The complete application passed all 34 automated tests during the observed local execution.

The project demonstrates how functional programming techniques and native interoperability can be combined in a practical C# application.