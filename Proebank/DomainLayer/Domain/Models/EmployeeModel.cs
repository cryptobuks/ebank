using System;

namespace DomainLayer.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public DateTime HiredOn { get; set; }

        public DateTime FiredOn { get; set; }

        public EmployeeRole EmployeeRole { get; set; }
    }
}
