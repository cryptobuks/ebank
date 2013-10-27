using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Interfaces
{
    public interface ICustomer
    {
        long Id { get; set; }

        string IdentificationNumber { get; set; }

        string LastName { get; set; }

        string FirstName { get; set; }

        string MiddleName { get; set; }

        string Email { get; set; }

        string Phone { get; set; }

        string Address { get; set; }

    }
}
