namespace Domain.Enums
{
    public enum EntryType : int
    {
        Accrual,    // Interest accruals
        Payment,    // Customer payments
        Transfer,   // From account to account
        Capital,    // Special for bank accounts
    }
}