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
        [RegularExpression("[a-zA-Z]{1,30}", ErrorMessage = "Not valid name")]
        public string LastName { get; set; }

        [DisplayName("First name")]
        [RegularExpression("[a-zA-Z]{1,30}", ErrorMessage = "Not valid name")]
        public string FirstName { get; set; }

        [DisplayName("Middle name")]
        [RegularExpression("[a-zA-Z]{1,30}", ErrorMessage = "Not valid name")]
        public string MiddleName { get; set; }

        
        [DisplayName("Date of birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }

        [DisplayName("Passport number")]
        [RegularExpression("([A-Z]{2}[0-9]{7})", ErrorMessage = "Enter only UPPERCASE latin characters and numbers")]
        public string Passport { get; set; }

        [DisplayName("Identification No.")]
        [RegularExpression("([0-9]{7}[A-Z][0-9]{3}[A-Z]{2}[0-9])", ErrorMessage = "Number should be like 0000000X000XX0")]
        public string Identification { get; set; }

        [DisplayName("Address")]
        public string Address { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
