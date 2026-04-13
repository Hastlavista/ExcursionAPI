using System;

namespace BlueDragon.Excursion.Core.Shared;

public class TradeLimitExceededException : Exception
{
    public TradeLimitExceededException() : base("Monthly trade limit reached. Upgrade to Pro.")
    {
    }
}