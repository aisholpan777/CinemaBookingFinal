
# Individual Contribution Report — Cinema Booking System

**Full name:** Zhuldyzay Galymzhankyzy  
**Student group:** IT-2512  
**Team members:** Aisholpan Bolatova, Dilnaz Momenzhanova, Zhuldyzay Galymzhankyzy, Zhasmin Askar  
**Project title:** Cinema Booking System  
**Course:** Introduction to Functional Programming  
**Academic year:** 2026–2027  
**Technology:** C# / .NET 8, LINQ, JSON, Native C

---

## 1. My Actual Contributions

My responsibility in the Cinema Booking System project is **Pricing Logic, Functional Composition, and Error Handling**.

My work focuses on calculating ticket prices, applying customer discounts and configurable pricing policies, combining functional operations, and handling successful and unsuccessful results.

The main files and methods related to my contribution are:

| Source File | Method or Component | Purpose |
|---|---|---|
| `Services/Pricing.cs` | `ConfigureDiscount()` | Creates a configurable discount function |
| `Services/Pricing.cs` | `ConfigureSurcharge()` | Creates a configurable surcharge function |
| `Services/Pricing.cs` | `CurryPolicy()` | Configures pricing policies using currying |
| `Services/Pricing.cs` | `ApplyPolicy()` | Applies the selected pricing policy |
| `Services/Pricing.cs` | `BuildQuote()` | Constructs a ticket quotation |
| `Services/Pricing.cs` | `QuoteBooking()` | Calculates a quotation for a booking request |
| `Services/Pricing.cs` | `QuoteBatch()` | Processes multiple quotation requests |
| `Services/Checkout.cs` | `QuoteWithNativeFee()` | Integrates pricing with the native booking fee |
| `Services/Checkout.cs` | `BookWithNativeFee()` | Combines quotation and booking confirmation |
| `Functional/Result.cs` | `Result<T>`, `Map()`, `Bind()` | Provides explicit success and failure handling |
| `Functional/Result.cs` | `Maybe<T>`, `Find()` | Handles optional values safely |

I developed configurable pricing functions, implemented functional composition, integrated pricing into the checkout workflow, and worked with explicit error-handling mechanisms.

The pricing system supports different customer categories and pricing policies.

### Customer Discounts

| Customer Type | Discount |
|---|---|
| Adult | 0% |
| Student | 20% |
| Child | 40% |

### Pricing Policies

| Policy | Additional Discount |
|---|---|
| Standard | 0% |
| Promo | 10% |

The booking fee is calculated separately using the native C module and is equal to **5% of the discounted ticket price**.

My contribution also includes testing pricing calculations, policy configuration, functional composition, failure propagation, and native checkout integration.

These components ensure that ticket prices are calculated consistently and that invalid operations are handled safely.

---

## 2. Functional Techniques

### 2.1 Pure Functions

Pricing calculations are separated from console interaction and file operations.

A pricing function receives input values and produces a calculated result without modifying the application's state.

This makes calculations predictable and suitable for automated testing.

### 2.2 Higher-Order Functions

The application uses higher-order functions that accept or return delegates such as `Func<T, TResult>`.

For example, `ConfigureDiscount()` creates a function that applies a configured discount percentage.

This allows pricing rules to be reused with different values.

Instead of creating separate pricing implementations for every discount, the application can configure the required behavior.

### 2.3 Currying and Partial Application

The method `CurryPolicy()` demonstrates functional configuration.

It allows the application to configure a pricing policy before applying it to a ticket price.

For example, the system can create separate functions for standard and promotional pricing.

```csharp
var standard = Pricing.CurryPolicy(0);
var promo = Pricing.CurryPolicy(10);
```

This improves code reuse and keeps business rules configurable.

### 2.4 Functional Composition

The pricing workflow combines multiple operations.

The application validates the booking input, applies the pricing policy, constructs a quotation, and integrates the booking fee.

The output of one operation becomes the input to the next.

This demonstrates a functional pipeline in which each stage has a clear responsibility.

### 2.5 Result, Map, and Bind

The application uses `Result<T>` to represent successful and unsuccessful operations.

A successful operation returns `Ok(value)`.

An unsuccessful operation returns `Fail(error)`.

The `Map()` method transforms a successful value without changing the failure case.

The `Bind()` method connects operations that can themselves return a `Result<T>`.

If an earlier operation fails, subsequent dependent operations are skipped.

This prevents invalid data from continuing through the pricing pipeline.

### 2.6 Maybe Type

The application also uses `Maybe<T>` to represent optional values.

`Some` indicates that a value exists, while `None` indicates that a value is missing.

The `Find()` operation demonstrates safe lookup behavior without relying on null values.

### 2.7 Immutability

Pricing functions calculate new values without modifying the original cinema state.

Booking confirmation creates an updated state only after all required calculations and validations succeed.

This helps preserve consistency when an operation fails.

---

## 3. Integration

The pricing module connects domain validation, configurable policies, native booking fees, and booking confirmation.

### 3.1 Ticket Quotation Workflow

The user selects option 6 — Quote ticket.

The application receives the session ID, seat number, customer type, and pricing policy.

The quotation process follows these steps:

1. The booking input is validated.
2. The appropriate customer discount is selected.
3. The configured pricing policy is applied.
4. The discounted ticket price is calculated.
5. The native C module calculates the booking fee.
6. The application constructs the final quotation.
7. The console displays the calculated ticket price.

This workflow demonstrates functional composition and explicit error handling.

### 3.2 Standard Pricing Example

Consider an adult customer booking a ticket with a base price of 2,500.00 KZT.

The standard policy applies no additional discount.

| Component | Amount |
|---|---|
| Base ticket price | 2,500.00 KZT |
| Customer discount | 0% |
| Policy discount | 0% |
| Discounted price | 2,500.00 KZT |
| Native booking fee (5%) | 125.00 KZT |
| Final total | 2,625.00 KZT |

### 3.3 Promotional Pricing Example

Consider the same adult customer using the promotional policy.

The promotional policy applies a 10% discount.

| Component | Amount |
|---|---|
| Base ticket price | 2,500.00 KZT |
| Customer discount | 0% |
| Policy discount | 10% |
| Discounted price | 2,250.00 KZT |
| Native booking fee (5%) | 112.50 KZT |
| Final total | 2,362.50 KZT |

These examples demonstrate how changing the pricing policy affects the final ticket quotation.

### 3.4 Native Fee Integration

The method `Checkout.QuoteWithNativeFee()` connects the pricing module with the native C fee calculator.

The native function receives the discounted ticket price in cents and the fee rate in basis points.

A rate of 500 basis points represents 5%.

The calculated fee is added to the discounted ticket price.

If native fee calculation fails, the checkout workflow returns an explicit failure.

### 3.5 Booking Integration

The method `Checkout.BookWithNativeFee()` connects ticket quotation with booking confirmation.

The booking is confirmed only when validation, pricing, and native fee calculation succeed.

If a required operation fails, the system does not create a confirmed booking.

This protects the original cinema state from invalid changes.

### 3.6 Integration with Other Modules

The pricing module depends on validated booking information from the domain models.

The console interface receives customer input and displays ticket quotations.

The native C module calculates booking fees.

The booking service confirms reservations using valid pricing results.

Together, these components provide a complete ticket pricing and checkout workflow.

---

## 4. Tests Relevant to My Responsibility

The application includes an automated test runner in `Tests/TestRunner.cs`.

The tests were executed using:

```bash
dotnet run -- --test
```

**Observed result:**

```text
TOTAL: 34 passed, 0 failed
```

The following tests verify pricing, functional composition, and error handling.

### Test 1 — Pricing Policies Differ

**Method:** `Pricing.CurryPolicy()`

**Expected result:** A ticket price of 10,000 cents produces different results under standard and promotional policies.

- Standard policy: 10,000 cents.
- Promotional policy: 9,000 cents.

**Actual result:** PASS.

### Test 2 — Student Discount

**Method:** `Pricing.ConfigureDiscount()`

**Expected result:** Applying a 20% student discount to 10,000 cents produces 8,000 cents.

**Actual result:** PASS.

### Test 3 — Map and Bind Success

**Methods:** `Map()`, `Bind()`

**Expected result:** A successful result containing 2 is transformed to 3 and then to 6.

**Actual result:** PASS.

This demonstrates functional composition using successful results.

### Test 4 — Failure Skips Subsequent Operations

**Methods:** `Map()`, `Bind()`

**Expected result:** When an operation returns `Fail`, subsequent dependent functions are not executed.

**Actual result:** PASS.

This verifies safe failure propagation.

### Test 5 — Maybe Found and Missing

**Methods:** `Maybe<T>`, `Find()`

**Expected result:** Searching for an existing movie returns `Some`, while searching for a missing movie returns `None`.

**Actual result:** PASS.

### Test 6 — Invalid Discount Rejected

**Method:** `Pricing.ConfigureDiscount()`

**Expected result:** Configuring an invalid discount of 120% throws `ArgumentOutOfRangeException`.

**Actual result:** PASS.

This verifies validation of pricing configuration.

### Test 7 — Native Checkout Workflow

**Method:** `Checkout.BookWithNativeFee()`

**Expected result:** A ticket price of 10,000 cents with a native fee of 500 cents produces a total of 10,500 cents and allows valid booking confirmation.

**Actual result:** PASS.

### Test 8 — Native Failure Preserves State

**Method:** `Checkout.BookWithNativeFee()`

**Expected result:** If native fee calculation fails, booking confirmation is rejected and the original cinema state remains unchanged.

**Actual result:** PASS.

### Testing Summary

| Test Category | Result |
|---|---|
| Standard and promotional policies | PASS |
| Customer discount calculations | PASS |
| Functional composition with Map and Bind | PASS |
| Failure propagation | PASS |
| Optional value handling | PASS |
| Invalid pricing configuration | PASS |
| Native checkout integration | PASS |
| Immutable state preservation | PASS |

The complete application passed all 34 automated tests during the observed local execution.

These tests verify the correctness of pricing calculations, configurable policies, functional error handling, and checkout integration.

---

## 5. Defect or Technical Challenge

### Challenge: Preventing Failed Pricing Operations from Affecting Bookings

An important technical challenge in the pricing module is ensuring that invalid calculations do not produce confirmed bookings.

The ticket quotation workflow includes multiple dependent operations, including validation, discount calculation, pricing policy application, native fee calculation, and booking confirmation.

If one operation fails, continuing the workflow could produce an invalid ticket price or inconsistent booking state.

I addressed this challenge by using explicit result types and functional composition.

The `Result<T>` type represents successful and unsuccessful operations.

The `Map()` and `Bind()` methods allow the application to connect dependent calculations while propagating failures.

When a calculation fails, subsequent operations are skipped.

The checkout workflow also checks the native fee result before confirming a booking.

If the native fee calculation fails, the operation returns a failure and preserves the original state.

These safeguards are verified by automated tests for failure propagation and native checkout failure.

The tests passed during the observed execution.

This approach improves reliability by ensuring that booking confirmation occurs only after successful validation and pricing.

---

## 6. Limitations and Shared Work

The Cinema Booking System is a team project with four main responsibility areas.

| Team Member | Responsibility |
|---|---|
| Aisholpan | Movies, Sessions, Console Interface, and System Integration |
| Dilnaz | Booking Management, Domain Validation, and JSON Persistence |
| Zhuldyzay | Pricing Logic, Functional Composition, and Error Handling |
| Zhasmin | LINQ Reports, Extension Methods, and Native C Integration |

My contribution focuses on configurable pricing, functional composition, quotation construction, explicit error handling, and checkout integration.

The pricing module depends on validated booking inputs and session information.

The native C module provides booking fee calculations.

The booking service uses successful pricing results to confirm reservations.

The console interface displays quotations and failure messages.

Together, these modules create a complete ticket pricing and booking workflow.

### Current Limitations

The application has several limitations:

- Pricing policies are configured in code rather than through an administrative interface.
- The application uses a console interface instead of a graphical interface.
- Real payment processing is not implemented.
- Data is stored locally in JSON files.
- Online multi-user booking synchronization is not supported.

Despite these limitations, the system demonstrates configurable pricing, higher-order functions, functional composition, explicit error handling, and immutable state transitions.

---

## 7. Defense Preparation

### 7.1 Normal Flow — Ticket Quotation

During the defense, I can demonstrate the ticket quotation functionality.

I start the application and select option 6.

I enter a valid session ID, seat number, customer type, and pricing policy.

The application validates the input, applies the appropriate discounts, calculates the native booking fee, and displays the final quotation.

I can explain how `ValidatedSelection.Create()`, `Pricing.ApplyPolicy()`, `Pricing.BuildQuote()`, and `Checkout.QuoteWithNativeFee()` work together.

### 7.2 Failure Flow — Invalid Discount

I can demonstrate validation by attempting to configure a discount greater than 100%.

For example, `Pricing.ConfigureDiscount(120)` is invalid.

The method throws `ArgumentOutOfRangeException`.

This prevents the application from creating an invalid pricing configuration.

### 7.3 Failure Flow — Native Fee Calculation

I can demonstrate a native fee calculation failure using the failing fee adapter.

When the native fee calculation returns `Fail`, the checkout operation is rejected.

The original cinema state remains unchanged.

This demonstrates functional error handling and safe state preservation.

### 7.4 Possible Live Code Change

A possible live coding task is changing the promotional discount from 10% to 15%.

I would update the pricing policy configuration:

```csharp
var promo = Pricing.CurryPolicy(15);
```

I would then verify the new pricing result and run the automated tests.

After making the change, I would execute:

```bash
dotnet build
dotnet run -- --test
```

This would demonstrate configurable pricing and the ability to modify a functional rule without rewriting the entire pricing workflow.

### 7.5 Functional Programming Concepts

**Pure function:** A function that calculates a result without modifying external state.

**Higher-order function:** A function that accepts another function as an argument or returns a function.

**Currying:** Transforming a function into a sequence of functions that receive arguments separately.

**Partial application:** Creating a specialized function by fixing some configuration values.

**Functional composition:** Combining functions so that the output of one operation becomes the input of another.

**Result type:** A type that explicitly represents success or failure.

**Map:** Applies a transformation to a successful result while preserving failures.

**Bind:** Connects dependent operations that may fail.

**Maybe type:** Represents the presence or absence of a value.

**Immutability:** Prevents direct modification of existing application state.

---

## 8. Conclusion

My contribution to the Cinema Booking System focuses on pricing logic, functional composition, and error handling.

I developed configurable pricing functions, implemented customer discounts and pricing policies, connected functional operations into a quotation pipeline, and integrated pricing with the checkout workflow.

This part of the project demonstrates important functional programming principles, including pure functions, higher-order functions, currying, partial application, functional composition, explicit error handling, and immutable state transitions.

The pricing module supports different customer categories and configurable policies.

The checkout workflow combines discounted ticket prices with native booking fee calculations while preventing failed operations from creating invalid bookings.

Automated tests verify pricing calculations, functional composition, failure propagation, invalid discount rejection, native checkout integration, and state preservation.

The complete application passed all 34 automated tests during the observed local execution.

The project demonstrates how functional programming techniques can be applied to implement reliable and configurable ticket pricing in a practical C# application.
