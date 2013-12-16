using System;
using System.Collections.Generic;
using System.Data;
using Domain.Enums;
using Domain.Models.Customers;

namespace Domain.Models.Loans
{
    /// <summary>
    /// Customer or his guarantor document
    /// </summary>
    public class Document : Entity
    {
        public string Number { get; set; }

        public DocType DocType { get; set; }

        public TariffDocType TariffDocType { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
