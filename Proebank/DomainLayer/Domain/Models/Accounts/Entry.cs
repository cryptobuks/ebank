using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Schema;
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

        public Guid Id { get; set; }

        public decimal Amount { get; set; }

        public Currency Currency { get; set; }

        public DateTime Date { get; set; }

        public EntryType Type { get; set; }

        public EntrySubType SubType { get; set; }

        public bool SameIdentityAs(Entry other)
        {
            return other != null && other.Id.Equals(Id);
        }

        public static Entry GetOppositeFor(Entry entry)
        {
            var opposite = new Entry
            {
                Amount = entry.Amount*-1,
                Currency = entry.Currency,
                Date = entry.Date,
                SubType = entry.SubType,
                Type = entry.Type
            };
            return opposite;
        }
    }
}
