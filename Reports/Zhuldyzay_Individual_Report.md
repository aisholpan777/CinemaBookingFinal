# Individual Contribution Report — Cinema Booking System

**Full name:** Zhuldyzay Galymzhankyzy
**Student group:** IT-2512
**Team members:** Aisholpan, Dilnaz, Zhuldyzay, Zhasmin  
**Project title:** Cinema Booking System  
**Course:** Introduction to Functional Programming  
**Academic year:** 2026–2027  
**Technology:** C# / .NET 8, LINQ, JSON, Native C

---

## 1. My Actual Contributions

My assigned responsibility in the Cinema Booking System project is **Pricing, Functional Composition, and Error Handling**.

This part of the application calculates ticket prices, applies customer discounts and configurable pricing policies, combines multiple functional operations, and handles successful and unsuccessful results.

The main files and methods associated with my responsibility are:

| Source file | Method or component | Purpose |
|---|---|---|
| `Services/Pricing.cs` | `ConfigureDiscount()` | Creates a configurable discount function |
| `Services/Pricing.cs` | `ConfigureSurcharge()` | Creates a configurable surcharge function |
| `Services/Pricing.cs` | `CurryPolicy()` | Configures a pricing policy using currying |
| `Services/Pricing.cs` | `ApplyPolicy()` | Applies the selected pricing policy |
| `Services/Pricing.cs` | `BuildQuote()` | Constructs a ticket quotation |
| `Services/Pricing.cs` | `QuoteBooking()` | Calculates a quotation for a booking request |
| `Services/Pricing.cs` | `QuoteBatch()` | Processes multiple quotation requests |
| `Services/Checkout.cs` | `QuoteWithNativeFee()` | Integrates pricing with the native booking fee |
| `Services/Checkout.cs` | `BookWithNativeFee()` | Combines quotation and booking confirmation |
| `Functional/Result.cs` | `Result<T>`, `Map()`, `Bind()` | Provides explicit success and failure handling |
| `Functional/Result.cs` | `Maybe<T>`, `Find()` | Handles optional values safely |

The pricing system supports different customer categories and pricing policies.

Customer discounts are:

- Adult: 0%
- Student: 20%
- Child: 40%

The application also supports two pricing policies:

- Standard: 0% additional discount
- Promo: 10% additional discount

The booking fee is calculated separately using the native C module and is equal to 5% of the discounted ticket price.

**Contribution clarification:** These components are assigned to me for the team defense. The integrated implementation was prepared centrally, so this responsibility assignment does not independently establish personal authorship of every method.

---

## 2. Functional Techniques

### 2.1 Pure Functions

Pricing calculations are separated from console interaction and file operations.

A pricing function receives input values and produces a calculated result without modifying the application's state.

This makes calculations predictable and suitable for automated testing.

### 2.2 Higher-Order Functions

The application uses higher-order functions that accept or return delegates such as `Func<T, TResult>`.

For example, `ConfigureDiscount()` creates a function that applies a configured discount percentage.

This allows the pricing rules to be reused with different values.

Instead of creating separate pricing implementations for every discount, the application can configure the required behavior.

### 2.3 Currying and Partial Application

The method `CurryPolicy()` demonstrates functional configuration.

It allows the application to configure a pricing policy before applying it to a ticket price.

For example, the system can create separate functions for standard and promotional pricing.

This improves code reuse and keeps business rules configurable.

### 2.4 Functional Composition

The pricing workflow combines multiple operations.

The application validates the booking input, applies the pricing policy, constructs a quotation, and integrates the booking fee.

The output of one operation becomes the input to the next.

This demonstrates a functional pipeline in which each stage has a clear responsibility.

### 2.5 Result, Map, and Bind

The application uses `Result<T>` to represent success and failure explicitly.

`Map()` transforms a successful value while preserving an existing failure.

`Bind()` connects dependent operations that may also fail.

If an earlier operation fails, later dependent operations are skipped.

This prevents invalid data from continuing through the pricing and booking workflow.

### 2.6 Maybe Type

The application uses `Maybe<T>` to represent optional values.

`Maybe<T>.Some` represents an existing value.

`Maybe<T>.None` represents a missing value.

This makes missing lookups explicit without relying on null values.

### 2.7 Immutability

Pricing operations produce quotation values without directly changing the existing cinema state.

This supports predictable calculations and allows pricing rules to be tested independently.

---

## 3. Integration

The pricing module connects booking validation, pricing policies, native fee calculation, and booking confirmation.

### 3.1 Ticket Quotation Workflow

The user selects option `6` — Quote ticket.

The application receives the session ID, seat number, customer type, and selected pricing policy.

The quotation workflow then:

1. Validates the booking input.
2. Retrieves the session ticket price.
3. Applies the customer discount.
4. Applies the configured pricing policy.
5. Calculates the booking fee.
6. Returns the final quotation.

The console displays the calculated price to the user.

### 3.2 Pricing Example

Consider a cinema session with a base ticket price of **2,500.00 KZT**.

For an adult customer using the standard policy:

| Component | Amount |
|---|---:|
| Base ticket price | 2,500.00 KZT |
| Customer discount | 0% |
| Policy discount | 0% |
| Discounted ticket price | 2,500.00 KZT |
| Native booking fee (5%) | 125.00 KZT |
| **Final total** | **2,625.00 KZT** |

For an adult customer using the promotional policy:

| Component | Amount |
|---|---:|
| Base ticket price | 2,500.00 KZT |
| Promotional discount | 10% |
| Discounted ticket price | 2,250.00 KZT |
| Native booking fee (5%) | 112.50 KZT |
| **Final total** | **2,362.50 KZT** |

These examples demonstrate how configurable pricing changes the final ticket cost.

### 3.3 Native Fee Integration

The method `Checkout.QuoteWithNativeFee()` combines ticket quotation with the native fee calculator.

The native library calculates the additional booking fee using a rate expressed in basis points.

A rate of `500` basis points represents 5%.

The final quotation includes the discounted ticket price and the calculated booking fee.

### 3.4 Booking Integration

The method `Checkout.BookWithNativeFee()` connects pricing with booking confirmation.

A successful quotation is used to create a booking.

If pricing or fee calculation fails, the booking is not confirmed.

This preserves the original application state.

### 3.5 Integration with Other Modules

The pricing module uses session prices defined in the domain models.

The booking module uses the resulting quotation to confirm a reservation.

The native C module calculates the booking fee.

The console interface displays the quotation and any error messages.

The JSON persistence module saves the updated cinema state after a successful booking.

---

## 4. Tests Relevant to My Responsibility

The project contains an automated test runner in `Tests/TestRunner.cs`.

The test suite was executed using:

`dotnet run -- --test`

**Observed result:**

`TOTAL: 34 passed, 0 failed`

The following tests verify pricing, functional composition, and error handling.

### Test 1 — Policies differ

**Method:** `Pricing.QuoteBooking()`

**Expected result:**

For a base price of 10,000 cents:

- Standard policy produces 10,000 cents.
- Promotional policy produces 9,000 cents.

**Actual result:** PASS.

This confirms that configurable pricing policies affect the quotation.

### Test 2 — Student discount

**Method:** `Pricing.QuoteBooking()`

**Expected result:**

A student customer receives a 20% discount.

A ticket priced at 10,000 cents produces a discounted total of 8,000 cents when no additional fee is applied.

**Actual result:** PASS.

### Test 3 — Map and Bind success

**Methods:** `Map()` and `Bind()`

**Expected result:**

The initial value is `2`.

`Map()` adds `1`, producing `3`.

`Bind()` multiplies the result by `2`, producing `6`.

The final result is `Result<int>.Ok` containing `6`.

**Actual result:** PASS.

### Test 4 — Map and Bind skip failure

**Methods:** `Map()` and `Bind()`

**Expected result:**

When the initial result is a failure, the transformation functions are not executed.

The counter remains `0`, and the final result remains a failure.

**Actual result:** PASS.

This demonstrates correct failure propagation.

### Test 5 — Maybe found and missing

**Method:** `Find()`

**Expected result:**

- An existing movie returns `Maybe<Movie>.Some`.
- A nonexistent movie returns `Maybe<Movie>.None`.

**Actual result:** PASS.

### Test 6 — Reject discount over 100 percent

**Method:** `Pricing.ConfigureDiscount()`

**Expected result:**

Attempting to configure a discount of 120% throws `ArgumentOutOfRangeException`.

**Actual result:** PASS.

This verifies validation of invalid discount percentages.

### Test 7 — Native checkout workflow

**Method:** `Checkout.BookWithNativeFee()`

**Expected result:**

A ticket priced at 10,000 cents with a 5% native fee produces a total of 10,500 cents.

The booking is successfully created.

**Actual result:** PASS.

### Test 8 — Native failure preserves state

**Method:** `Checkout.BookWithNativeFee()`

**Expected result:**

When the fee calculator fails, the checkout operation returns a failure.

No booking is added, and the original state remains unchanged.

**Actual result:** PASS.

### Testing Summary

| Test name | Result |
|---|---|
| Policies differ | PASS |
| Student discount | PASS |
| Map and Bind success | PASS |
| Map and Bind skip failure | PASS |
| Maybe found and missing | PASS |
| Reject discount over 100 percent | PASS |
| Native checkout workflow | PASS |
| Native failure preserves state | PASS |

All 34 automated tests passed during the observed local execution.

These tests verify functionality relevant to my assigned area; the test runner alone does not establish individual test authorship.

---

## 5. Defect or Technical Challenge

### Challenge: Preventing Failure Propagation from Producing Invalid Results

An important technical challenge in the pricing and checkout workflow is handling failures without allowing invalid operations to continue.

For example, if a booking request contains invalid information, the application must not continue to confirm a reservation.

Similarly, if the native fee calculator fails, the system must not create a booking using an incomplete quotation.

The implementation addresses this challenge through `Result<T>`, `Map()`, and `Bind()`.

These functional operations preserve failures and prevent dependent stages from executing after an unsuccessful result.

The automated test `Map and Bind skip failure` verifies that transformation functions are not executed after a failure.

The test `Native failure preserves state` verifies that a failed fee calculation does not create a booking.

Both tests passed.

This is a verified technical challenge addressed by the implementation, rather than evidence of a particular student's personal debugging history.

---

## 6. Limitations and Shared Work

The Cinema Booking System has four assigned responsibility areas.

| Team member | Responsibility |
|---|---|
| Aisholpan | Movies, Sessions, and Console Interface |
| Dilnaz | Booking, State Validation, and JSON Persistence |
| Zhuldyzay | Pricing, Functional Composition, and Error Handling |
| Zhasmin | LINQ Reports, Extension Methods, and Native C Integration |

My assigned responsibility focuses on ticket pricing, functional composition, and error handling.

The pricing module depends on shared domain models, booking validation, native fee calculation, and console interaction.

The integrated project implementation was prepared centrally and divided into responsibility areas for the team defense.

### Current Limitations

The application has several limitations:

- It uses a console interface instead of a graphical interface.
- Pricing policies are configured in application logic rather than through an external administration system.
- It does not integrate with a real payment provider.
- Data is stored locally in JSON files.
- Online multi-user booking synchronization is not supported.

Despite these limitations, the application demonstrates configurable business rules, higher-order functions, functional composition, and explicit error handling.

---

## 7. Defense Preparation

### 7.1 Normal Flow — Ticket Quotation

During the defense, I can demonstrate ticket quotation.

I start the application and select option `6`.

I enter a valid session ID, seat number, customer type, and pricing policy.

The system calculates the discounted ticket price and adds the native booking fee.

I can explain how `Pricing.QuoteBooking()` and `Checkout.QuoteWithNativeFee()` work together.

### 7.2 Failure Flow — Invalid Discount

I can demonstrate a validation failure using an invalid discount percentage.

For example, calling `Pricing.ConfigureDiscount(120)` throws `ArgumentOutOfRangeException`.

The application rejects this invalid configuration.

I can explain why discount percentages must be validated before they are used in pricing calculations.

### 7.3 Failure Flow — Native Fee Error

Another failure example occurs when the native fee calculator cannot produce a valid result.

The checkout workflow returns `Result<CinemaState>.Fail`.

No booking is created.

This demonstrates safe error propagation and preservation of immutable state.

### 7.4 Possible Live Code Change

A possible live coding task is adding a new pricing policy.

For example, the instructor may request a 15% promotional discount.

I would use the existing configurable pricing functions to create the new policy and add an automated test verifying the expected quotation.

After making the change, I would execute:

```bash
dotnet build
dotnet run -- --test
```

This would verify that the new pricing rule works without breaking the existing functionality.

### 7.5 Functional Programming Concepts

**Pure function:** A function that calculates a result without changing external state.

**Higher-order function:** A function that accepts another function as an argument or returns a function.

**Currying:** Transforming a function into a sequence of functions that accept arguments in stages.

**Partial application:** Fixing some arguments of a function to create a more specialized function.

**Map:** Transforms a successful value while preserving failures.

**Bind:** Connects dependent operations that may return success or failure.

**Maybe:** Represents an existing or missing value explicitly.

**Immutability:** Data is not modified in place during calculations.

---

## 8. Conclusion

My assigned responsibility in the Cinema Booking System focuses on pricing calculations, functional composition, and error handling.

This part of the project demonstrates pure functions, higher-order functions, currying, partial application, immutable data, and explicit success and failure handling.

The pricing module supports different customer discounts and configurable pricing policies.

The checkout module integrates ticket pricing with native fee calculation and booking confirmation.

Automated tests verify pricing differences, discounts, functional composition, failure propagation, and checkout behavior.

The complete application passed all 34 automated tests during the observed local execution.

The project demonstrates how functional programming techniques can make business calculations more predictable, reusable, and reliable.