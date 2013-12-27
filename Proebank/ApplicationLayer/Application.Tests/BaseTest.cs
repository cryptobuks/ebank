using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AutoMoq;
using Domain;
using Domain.Contexts;
using Domain.Models.Accounts;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Practices.Unity;
using Moq;

namespace Application.Tests
{
    public class BaseTest
    {
        protected IUnityContainer Unity { get; set; }
        protected AutoMoqer Mocker { get; set; }

        protected BaseTest()
        {
            Unity = UnityConfig.GetConfiguredContainer();
        }
    }

    //public class FakeDataContext : DataContext
    //{
        
    //    private ObservableCollection<Account> _accounts;
    //    public override DbSet<Account> Accounts { get {return }}
    //}
}
