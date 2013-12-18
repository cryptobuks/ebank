using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Domain;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Presentation.Extensions
{
    public static class CustomerHelper
    {
        public static string GenerateName(UserManager<IdentityUser> userMgr, PersonalData personalData)
        {
            var sb = new StringBuilder((personalData.FirstName[0] + personalData.LastName).ToLowerInvariant());
            if (userMgr.FindByName(sb.ToString()) == null) return sb.ToString();
            var rnd = new Random();
            var suffix = rnd.Next(100000).ToString("x");
            while ((userMgr.FindByName(sb + suffix) != null))
            {
                suffix = rnd.Next(100000).ToString("x");
            }
            return sb.Append(suffix).ToString();
        }
    }
}