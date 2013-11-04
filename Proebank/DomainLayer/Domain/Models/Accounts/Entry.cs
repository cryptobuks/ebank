using System;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Models.Accounts
{
    /// <summary>
    /// Какая-либо операция со счётом (Account): начисление платежей или процентов
    /// </summary>
    public class Entry
    {
        public Entry()
        {
            Id = Guid.NewGuid();
        }

        public readonly Guid Id;

        public decimal Amount { get; set; }

        public Currency Currency { get; set; }

        public DateTime Date { get; set; }

        public EntryType Type { get; set; }

        public EntrySubType SubType { get; set; }

        public bool SameIdentityAs(Entry other)
        {
            return other != null && other.Id.Equals(Id);
        }
    }
}
