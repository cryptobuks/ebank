using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Schema;
using Domain.Enums;

namespace Domain.Models.Accounts
{
    /// <summary>
    /// Какая-либо операция со счётом (Account): начисление платежей или процентов
    /// </summary>
    public class Entry : Entity
    {
        public decimal Amount { get; set; }

        public Currency Currency { get; set; }

        public DateTime Date { get; set; }

        public EntryType Type { get; set; }

        public EntrySubType SubType { get; set; }

        public bool SameIdentityAs(Entry other)
        {
            return other != null && other.Id.Equals(Id);
        }

        public static void GetOppositeFor(Entry entry, Entry destiny)
        {
            destiny.Amount = entry.Amount*-1;
            destiny.Currency = entry.Currency;
            destiny.Date = entry.Date;
            destiny.SubType = entry.SubType;
            destiny.Type = entry.Type;
        }
    }
}
