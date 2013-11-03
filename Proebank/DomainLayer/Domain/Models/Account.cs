using Domain.Enums;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Domain.Models
{
    /// <summary>
    /// Банковский счёт
    /// </summary>
    public class Account
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
    }
}
