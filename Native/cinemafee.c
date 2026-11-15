
#include <stdint.h>
#include <limits.h>


int64_t cinema_fee(
    int64_t price_cents,
    int32_t rate_basis_points)
{
    
    if (price_cents < 0 ||
        rate_basis_points < 0 ||
        rate_basis_points > 10000)
    {
        return -1;
    }

    if (price_cents == 0 || rate_basis_points == 0)
    {
        return 0;
    }

    
    int64_t whole = price_cents / 10000;
    int64_t remainder = price_cents % 10000;

    int64_t whole_fee =
        whole * rate_basis_points;

    
    
    int64_t partial =
        remainder * rate_basis_points;

    
    int64_t remainder_fee =
        partial / 10000 +
        (partial % 10000 != 0 ? 1 : 0);

    
    if (whole_fee > INT64_MAX - remainder_fee)
    {
        return -1;
    }

    return whole_fee + remainder_fee;
}
