﻿using System;
using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Users
{
    /// <summary>
    /// Работник банка
    /// </summary>
    public class Employee
    {
        public Employee()
        {
            Id = Guid.NewGuid();
        }

        public readonly Guid Id;

        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public DateTime HiredOn { get; set; }

        public DateTime FiredOn { get; set; }

        public EmployeeRole EmployeeRole { get; set; }
    }
}