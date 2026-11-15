namespace CinemaBookingFinal;

public static class Checkout
{
    public static Result<Quote> QuoteWithNativeFee(CinemaState state, BookingInput input,
        Func<long, long> policy, IFeeCalculator native, int feeBasisPoints)
    {
        return ValidatedSelection.Create(state, input)
            .Bind(s => Pricing.ApplyPolicy(s, policy))
            .Bind(p => native.Calculate(p.DiscountedCents, feeBasisPoints)
                .Bind(fee => Pricing.BuildQuote(p, fee)));
    }

    public static Result<CinemaState> BookWithNativeFee(CinemaState state, BookingInput input,
        Func<long, long> policy, IFeeCalculator native, int feeBasisPoints, DateTime now) =>
        QuoteWithNativeFee(state, input, policy, native, feeBasisPoints)
            .Bind(q => BookingService.Confirm(state, q, now));
}