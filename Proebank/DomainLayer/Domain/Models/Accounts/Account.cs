using System;
using System.Collections.Generic;
using Domain.Enums;
using Domain.Shared;

namespace Domain.Models.Accounts
{
    /// <summary>
    /// Банковский счёт
    /// </summary>
    public class Account : IEntity<Account>
    {
        public Guid Id { get; set; }

        public AccountType Type { get; set; }

        /// <summary>
        /// Порядковый номер счёта внутри типа
        /// </summary>
        public int Number { get; set; }

        public string NumberText 
        { 
            get { return String.Concat(((int)Type).ToString(), Number.ToString("D9")); } 
        }

        public Currency Currency { get; set; }

        public virtual ICollection<Entry> Entries { get; set; }

        public DateTime DateOpened { get; set; }

        public DateTime? DateClosed { get; set; }

        public bool IsClosed { get; set; }

        public bool SameIdentityAs(Account other)
        {
            return other != null && other.Id.Equals(Id);
        }
    }
}
