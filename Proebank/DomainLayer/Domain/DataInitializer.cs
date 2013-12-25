using System;
using System.Configuration;
using System.Data.Entity;
using Domain.Migrations;

namespace Domain
{
    public class DataInitializer : MigrateDatabaseToLatestVersion<DataContext, Configuration>
    {
    }
}
