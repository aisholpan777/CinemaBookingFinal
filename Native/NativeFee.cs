
using System;
using System.Runtime.InteropServices;

namespace CinemaBookingFinal;

public interface IFeeCalculator
{
    Result<long> Calculate(long priceCents, int basisPoints);
}

public sealed class NativeFeeCalculator : IFeeCalculator
{
    [DllImport(
        "cinemafee",
        EntryPoint = "cinema_fee",
        CallingConvention = CallingConvention.Cdecl)]
    private static extern long CinemaFee(
        long priceCents,
        int basisPoints);

    public Result<long> Calculate(
        long priceCents,
        int basisPoints)
    {
        
        if (priceCents < 0)
            return Result.Fail<long>(
                "Invalid price for native fee");

        if (basisPoints < 0 || basisPoints > 10000)
            return Result.Fail<long>(
                "Invalid fee basis points");

        try
        {
            long fee = CinemaFee(
                priceCents,
                basisPoints);

            if (fee < 0)
                return Result.Fail<long>(
                    "Native fee calculation rejected inputs");

            
            
            if (fee > priceCents)
                return Result.Fail<long>(
                    "Invalid native fee result");

            return Result.Ok(fee);
        }
        catch (DllNotFoundException ex)
        {
            return Result.Fail<long>(
                "Native library missing: " + ex.Message);
        }
        catch (EntryPointNotFoundException ex)
        {
            return Result.Fail<long>(
                "Native function missing: " + ex.Message);
        }
        catch (BadImageFormatException ex)
        {
            return Result.Fail<long>(
                "Native library architecture mismatch: " +
                ex.Message);
        }
        catch (OverflowException)
        {
            return Result.Fail<long>(
                "Native fee calculation overflow");
        }
    }
}


public sealed class FailingFeeCalculator : IFeeCalculator
{
    public Result<long> Calculate(
        long priceCents,
        int basisPoints)
    {
        return Result.Fail<long>(
            "Native adapter unavailable");
    }
}
