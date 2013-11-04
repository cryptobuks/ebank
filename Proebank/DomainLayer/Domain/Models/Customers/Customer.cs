using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Customers
{
    /// <summary>
    /// Клиент банка
    /// </summary>
    public class Customer : IdentityUser
    {
        public string IdentificationNumber { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }
    }
}
