using System;
using Domain.Enums;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Users
{
    /// <summary>
    /// Работник банка
    /// </summary>
    public class Employee : IdentityUser
    {
        //public DateTime HiredOn { get; set; }

        //public DateTime FiredOn { get; set; }

        public EmployeeRole EmployeeRole { get; set; }

        public int Level { get; set; }
    }
}
