namespace CrossCutting.Enums
{
    public enum EntryType : int
    {
        Accrual,
        Payment,
    }

    public enum EntrySubType : int
    {
        GeneralDebt,
        Interest,
        Fine,
    }
}