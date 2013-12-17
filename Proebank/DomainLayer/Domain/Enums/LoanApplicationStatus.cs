namespace Domain.Enums
{
    public enum LoanApplicationStatus : int
    {
        New,    // filled on site
        Filled,  // filled in bank
        UnderRiskConsideration,  // after filling of documents details application goes to security (risks consideration) service
        UnderCommitteeConsideration,  // special consideration
        Approved,   // can sign contract - positive decision from security service
        Rejected,   // application doesn't meet requirements
        Annuled,    // too long in approved
        Contracted, // completed
    }
}