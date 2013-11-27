namespace Domain.Enums
{
    public enum LoanApplicationStatus : int
    {
        New,    // filled on site
        InitiallyApproved,  // can go to bank to complete application
        UnderRiskConsideration,  // after filling of documents details application goes to security (risks consideration) service
        Approved,   // can sign contract - positive decision from security service
        Rejected,   // application doesn't meet requirements
        Annuled,    // too long in approved
        Contracted, // completed
    }
}