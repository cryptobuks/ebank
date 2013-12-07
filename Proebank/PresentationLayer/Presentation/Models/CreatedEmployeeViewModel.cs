using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Models.Users;

namespace Presentation.Models
{
    public class CreatedEmployeeViewModel
    {
        public Employee Employee { get; set; }

        public string Password { get; set; }
    }
}