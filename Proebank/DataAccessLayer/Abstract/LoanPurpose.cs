using System;

namespace DataAccessLayer.Abstract
{
    [Flags]
    public enum LoanPurpose
    {
        Common = 0,
        Car,
        Education,
        Housing,
    }
}
