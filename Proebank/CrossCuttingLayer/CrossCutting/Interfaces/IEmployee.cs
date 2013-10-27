using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Interfaces
{
    public interface IEmployee
    {
        int Id { get; set; }

        string LastName { get; set; }

        string FirstName { get; set; }

        string MiddleName { get; set; }

        DateTime HiredOn { get; set; }

        DateTime FiredOn { get; set; }

        EmployeeRole EmployeeRole { get; set; }
    }
}
