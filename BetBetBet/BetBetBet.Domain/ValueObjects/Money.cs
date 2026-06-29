using System.Globalization;
using BetBetBet.Domain.Common;

namespace BetBetBet.Domain.ValueObjects;

public sealed record Money : IComparable<Money>
{
    public decimal Amount { get; }

    private Money(decimal amount) => Amount = amount;

    public static Result<Money> Create(decimal amount)
    {
        if (amount < 0)
            return new Error("Money.NegativeAmount", "Amount cannot be negative.");
        return new Money(amount);
    }

    public int CompareTo(Money? other) => other is null ? 1 : Amount.CompareTo(other.Amount);

    public static Money operator +(Money left, Money right) => new(left.Amount + right.Amount);
    public static Money operator -(Money left, Money right) => new(left.Amount - right.Amount);

    public static bool operator >(Money left, Money right) => left.CompareTo(right) > 0;
    public static bool operator <(Money left, Money right) => left.CompareTo(right) < 0;
    public static bool operator >=(Money left, Money right) => left.CompareTo(right) >= 0;
    public static bool operator <=(Money left, Money right) => left.CompareTo(right) <= 0;

    public override string ToString() => Amount.ToString(CultureInfo.InvariantCulture);
}
