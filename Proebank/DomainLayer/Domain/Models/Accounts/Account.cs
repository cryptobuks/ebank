using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using Domain.Enums;

namespace Domain.Models.Accounts
{
    /// <summary>
    /// Банковский счёт
    /// </summary>
    public class Account
    {
        public Account()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public AccountType Type { get; set; }

        /// <summary>
        /// Порядковый номер счёта внутри типа
        /// </summary>
        public int Number { get; set; }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Computed)]
        public string NumberText 
        { 
            get { return String.Concat(((int)Type).ToString(CultureInfo.InvariantCulture), Number.ToString("D9")); } 
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

        // TODO: check if we need it for readonly property
        [NotMapped]
        public decimal Balance
        {
            get { return Entries.Sum(e => e.Amount); }
        }
    }
}
