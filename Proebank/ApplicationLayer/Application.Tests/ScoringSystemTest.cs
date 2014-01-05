using System;
using System.Diagnostics;
using Domain.Enums;
using Domain.Models.Loans;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Application.Tests
{
    /// <summary>
    /// Summary description for ScoringSystemTest
    /// </summary>
    [TestClass]
    public class ScoringSystemTest
    {
        //private static LoanApplication _loanApplication1;
        //private static LoanApplication _loanApplication2;
        //private static LoanApplication _loanApplication3;
        //private static LoanApplication _worstLoanApplication;
        

        //[ClassInitialize]
        //public static void InitAttributes(TestContext context)
        //{
        //    var personalData1 = new PersonalData
        //    {
        //        DateOfBirth = new DateTime(1972, 10, 17),
        //    };


        //    //loanApplications

        //    _loanApplication1 = new LoanApplication
        //    {
        //        PersonalData = personalData1,
        //        ChildrenCount = 2,
        //        MiddleIncome = 5000000,
        //        HigherEducation = Education.Higher,
        //        LengthOfWork = 5,
        //        IsHomeowner = true,
        //        IsMarried = MaritalStatus.Married
        //    };

        //    _loanApplication2 = new LoanApplication
        //    {
        //        PersonalData = personalData1,
        //        ChildrenCount = 4,
        //        MiddleIncome = 3000000,
        //        HigherEducation = Education.HigherIncomplete,
        //        LengthOfWork = 1,
        //        IsHomeowner = false,
        //        IsMarried = MaritalStatus.Married
        //    };

        //    _loanApplication3 = new LoanApplication()
        //    {
        //        PersonalData = personalData1,
        //        ChildrenCount = 2,
        //        MiddleIncome = 5000000,
        //        HigherEducation = Education.Higher,
        //        LengthOfWork = 5,
        //        IsHomeowner = false,
        //        IsMarried = MaritalStatus.Married
        //    };

        //    _worstLoanApplication = new LoanApplication()
        //    {
        //        PersonalData = personalData1,
        //        ChildrenCount = 4,
        //        MiddleIncome = 2000000,
        //        HigherEducation = Education.LowerSecondary,
        //        LengthOfWork = 1,
        //        IsHomeowner = false,
        //        IsMarried = MaritalStatus.Divorced
        //    };


        //}


        //[TestMethod]
        //public void CalculatePredictions()
        //{
        //var p1 = ScoringSystem.CalculatePredictions(_loanApplication1);
        //var p2 = ScoringSystem.CalculatePredictions(_loanApplication2);
        //var p3 = ScoringSystem.CalculatePredictions(_loanApplication3);
        //var p4 = ScoringSystem.CalculatePredictions(_worstLoanApplication);
        //Debug.WriteLine(p1);
        //Debug.WriteLine(p2);
        //Debug.WriteLine(p3);
        //Debug.WriteLine(p4);
        //}
    }
}
