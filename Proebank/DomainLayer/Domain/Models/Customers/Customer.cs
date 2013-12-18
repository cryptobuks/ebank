using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models.Loans;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain.Models.Customers
{
    /// <summary>
    /// Клиент банка
    /// </summary>
    public class Customer : IdentityUser
    {
        [Required]
        public virtual PersonalData PersonalData { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }
}
