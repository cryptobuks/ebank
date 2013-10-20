using System;

namespace DataAccessLayer.Abstract
{
    public abstract class Tariff
    {
        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public decimal InterestRate
        {
            get;
            set;
        }

        public decimal MinAmount
        {
            get;
            set;
        }

        public decimal MaxAmount
        {
            get;
            set;
        }

        public DateTime CreationDate
        {
            get;
            set;
        }

        public DateTime EndDate
        {
            get;
            set;
        }

        public int MinTerm
        {
            get;
            set;
        }

        public int MaxTerm
        {
            get;
            set;
        }

        public bool IsGuarantorNeeded
        {
            get;
            set;
        }

        public short MinAge
        {
            get;
            set;
        }

        public short MaxAge
        {
            get;
            set;
        }

        public decimal InitialFee
        {
            get;
            set;
        }

        public bool IsSecondaryDocumentNeeded
        {
            get;
            set;
        }

        public virtual LoanPurpose LoanPurpose
        {
            get;
            set;
        }
    }
}
