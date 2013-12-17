using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Domain.Models.Customers;

namespace Domain.Models.Loans
{
    /// <summary>
    /// Customer or his guarantor document
    /// </summary>
    public class PersonalData : Entity
    {
        [DisplayName("Last name")]
        [RegularExpression("[a-zA-Z]{2,30}", ErrorMessage = "Not valid name")]
        public string LastName { get; set; }

        [DisplayName("First name")]
        [RegularExpression("[a-zA-Z]{2,30}", ErrorMessage = "Not valid name")]
        public string FirstName { get; set; }

        [DisplayName("Middle name")]
        [RegularExpression("[a-zA-Z]{2,30}", ErrorMessage = "Not valid name")]
        public string MiddleName { get; set; }

        public DateTime? DateOfBirth { get; set; }

        [DisplayName("Passport")]
        [RegularExpression("([A-Z]{2}[0-9]{7})", ErrorMessage = "Enter only UPPERCASE latin characters and numbers")]
        public string Passport { get; set; }

        [DisplayName("Identification No.")]
        [RegularExpression("([A-Z0-9]{14})", ErrorMessage = "Enter only UPPERCASE latin characters and numbers")]
        public string Identification { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
