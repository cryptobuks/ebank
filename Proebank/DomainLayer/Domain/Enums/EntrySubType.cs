namespace Domain.Enums
{
    public enum EntrySubType : int
    {
        CharterCapital,     // for bank accounts - it's initialization
        ContractService,    // main account for loan
        GeneralDebt,        // general debt
        Interest,           // accrued interest
        Fine,               // delay in payments
        BankLoanIssued,     // bank signed loan and gave money to customer
        BankLoanFromContract// money from contract account
    }
}