
using System;
using System.Collections.Generic;
using System.Linq;

namespace CinemaBookingFinal;

public static class Pricing
{
    
    public static Func<long, long> ConfigureDiscount(
        int percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentOutOfRangeException(
                nameof(percentage));

        return cents =>
        {
            if (cents < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(cents));

            decimal result =
                (decimal)cents * (100 - percentage) / 100m;

            return checked((long)decimal.Truncate(result));
        };
    }

    
    public static Func<long, long> ConfigureSurcharge(
        long surchargeCents)
    {
        if (surchargeCents < 0)
            throw new ArgumentOutOfRangeException(
                nameof(surchargeCents));

        return cents =>
        {
            if (cents < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(cents));

            return checked(cents + surchargeCents);
        };
    }

    
    public static Func<int, Func<long, long>> CurriedDiscount()
    {
        return percentage => ConfigureDiscount(percentage);
    }

    
    public static Func<long, long> CurryPolicy(
        int discountPercentage) =>
        CurriedDiscount()(discountPercentage);

    
        public static Result<PricedSelection> ApplyPolicy(
        ValidatedSelection selection,
        Func<long, long> policy)
    {
        if (selection is null)
            return Result.Fail<PricedSelection>(
                "Validated selection missing");

        if (policy is null)
            return Result.Fail<PricedSelection>(
                "Pricing policy missing");

        if (selection.BasePriceCents < 0)
            return Result.Fail<PricedSelection>(
                "Invalid base price");

        int customerDiscount = selection.CustomerType switch
        {
            "adult" => 0,
            "student" => 20,
            "child" => 40,
            _ => -1
        };

        if (customerDiscount < 0)
            return Result.Fail<PricedSelection>(
                "Invalid customer type");

        try
        {
            decimal customerAmount =
                (decimal)selection.BasePriceCents *
                (100 - customerDiscount) / 100m;

            long customerPrice =
                checked((long)decimal.Truncate(customerAmount));

            long finalPrice = policy(customerPrice);

            if (finalPrice < 0)
                return Result.Fail<PricedSelection>(
                    "Negative calculated price");

            return Result.Ok(
                new PricedSelection(selection, finalPrice));
        }
        catch (OverflowException)
        {
            return Result.Fail<PricedSelection>(
                "Price overflow");
        }
        catch (ArgumentOutOfRangeException)
        {
            return Result.Fail<PricedSelection>(
                "Invalid pricing value");
        }
    }

    
    public static Result<Quote> BuildQuote(
        PricedSelection priced,
        long feeCents)
    {
        if (priced is null)
            return Result.Fail<Quote>(
                "Priced selection missing");

        if (priced.Selection is null)
            return Result.Fail<Quote>(
                "Validated selection missing");

        if (priced.DiscountedCents < 0)
            return Result.Fail<Quote>(
                "Invalid discounted price");

        if (feeCents < 0)
            return Result.Fail<Quote>(
                "Invalid booking fee");

        try
        {
            long total = checked(
                priced.DiscountedCents + feeCents);

            return Result.Ok(new Quote(
                priced.Selection.SessionId,
                priced.Selection.Seat,
                priced.Selection.CustomerType,
                priced.DiscountedCents,
                feeCents,
                total));
        }
        catch (OverflowException)
        {
            return Result.Fail<Quote>(
                "Total price overflow");
        }
    }

    
    public static Result<Quote> QuoteBooking(
        CinemaState state,
        BookingInput input,
        Func<long, long> policy,
        long feeCents)
    {
        if (state is null)
            return Result.Fail<Quote>(
                "Cinema state missing");

        if (input is null)
            return Result.Fail<Quote>(
                "Booking input missing");

        if (policy is null)
            return Result.Fail<Quote>(
                "Pricing policy missing");

        if (feeCents < 0)
            return Result.Fail<Quote>(
                "Invalid booking fee");

        return ValidatedSelection.Create(state, input)
            .Bind(selection =>
                ApplyPolicy(selection, policy))
            .Bind(priced =>
                BuildQuote(priced, feeCents));
    }

    
    public static IReadOnlyList<Result<Quote>> QuoteBatch(
        CinemaState state,
        IEnumerable<BookingInput> inputs,
        Func<long, long> policy,
        long feeCents)
    {
        if (inputs is null)
        {
            return new[]
            {
                Result.Fail<Quote>(
                    "Booking inputs missing")
            };
        }

        return inputs
            .Select(input =>
                QuoteBooking(
                    state,
                    input,
                    policy,
                    feeCents))
            .ToArray();
    }
}
