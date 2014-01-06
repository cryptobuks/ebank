using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Loans;
using Domain.Enums;

namespace Application
{
    public class ScoringSystem
    {
        //MiddleIncome
        private const double W_MIDDLE_INCOME_MAX_VALUE = 30;

        //Children Count
        private const double W_CHILDREN_COUNT_ZERO = 20;
        private const double W_CHILDREN_COUNT_ONE = 15;
        private const double W_CHILDREN_COUNT_TWO = 10;
        private const double W_CHILDREN_COUNT_THREE = 5;
        private const double W_CHILDREN_COUNT_MORE_THREE = 0;

        //Education
        private const double W_EDUCATION_MORE_HIGHER = 10;
        private const double W_EDUCATION_HIGHER = 8;
        private const double W_EDUCATION_HIGHER_INCOMPLETE = 5;
        private const double W_EDUCATION_VOCATIONAL = 5;
        private const double W_EDUCATION_SECONDARY = 4;
        private const double W_EDUCATION_LOWER_SECONDARY = 2;

        //Is Married
        private const double W_IS_MARRIED = 10;
        private const double W_IS_NOT_MARRIED = 3;

        //Length of work
        private const double W_LENGTH_OF_WORK_LESS_ONE_YEAR = 1;
        private const double W_LENGTH_OF_WORK_BETWEEN_ONE_TWO_YEARS = 3;
        private const double W_LENGTH_OF_WORK_BETWEEN_TWO_THREE_YEARS = 6;
        private const double W_LENGTH_OF_WORK_BETWEEN_THREE_FOUR_YEARS = 7;
        private const double W_LENGTH_OF_WORK_BETWEEN_FOUR_FIVE_YEARS = 8;
        private const double W_LENGTH_OF_WORK_MORE_FIVE_YEARS = 10; 

        //Is Homeowner
        private const double W_IS_HOMEOWNER = 10;
        private const double W_IS_NOT_HOMEOWNER = 0;

        //Age
        private const double W_AGE_BETWEEN_20_25 = 3;
        private const double W_AGE_BETWEEN_25_60 = 10;
        private const double W_AGE_MORE_60 = 4;


        public static double CalculateRating(LoanApplication loanApplication, IEnumerable<LoanHistory> loanHistories)
        {
            try
            {
                return CalculateRatingOfParams(loanApplication) * 
                    ((CalculateCreditHistory(loanHistories) + CalculateDependencySalaryToLoanApplication(loanApplication))/2);
            }
            catch
            {
                return 0;
            }
            
        }


        private static double CalculateDependencySalaryToLoanApplication(LoanApplication loanApplication)
        {
            //Currency Rates is better take from web.config?? Or some global variables(for example Head should set up currency rates in the beginning of the day)
            decimal currency;
            switch (loanApplication.Currency)
            {
                case Currency.EUR:
                    currency = 13000;
                    break;
                case Currency.USD:
                    currency = 9560;
                    break;
                default:
                    currency = 1;
                    break;
            }
            var sumInMonth = (loanApplication.LoanAmount* currency)/loanApplication.Term;
            var k = sumInMonth/((decimal) 0.4 * loanApplication.MiddleIncome);
            if (k >= 1)
                throw  new Exception("Loan sum in month is bigger than 40% of Salary!!");
            return Convert.ToDouble(1 - k);
        }


        private static double CalculateCreditHistory(IEnumerable<LoanHistory> loanHistories)
        {
            if (loanHistories != null)
            {
                double countOfGoodLoans = 0;
                double amountOfLoans = 0;
                foreach (var loanHistory in loanHistories)
                {
                    amountOfLoans++;
                    if (!loanHistory.HadProblems)
                    {
                        countOfGoodLoans++;
                    }
                }
                return countOfGoodLoans/amountOfLoans;
            }
            //if (loanHistories == null)
            return 0.8;
        }

        private static double CalculateRatingOfParams(LoanApplication loanApplication)
        {
            const double w0 = -5;
            try
            {
                return (w0
                       + ConvertParamCashIncomeLevelToDouble(loanApplication.MiddleIncome)
                       + ConvertParamChildrenCountToDouble(loanApplication.ChildrenCount)
                       + ConvertParamHigherEducationToDouble(loanApplication.HigherEducation)
                       + ConvertParamIsMaridToDouble(loanApplication.IsMarried)
                       + ConvertParamLenghtOfWorkToDouble(loanApplication.LengthOfWork)
                       + ConvertParamIsHomeowner(loanApplication.IsHomeowner)
                       +
                       (loanApplication.PersonalData.DateOfBirth != null
                            ? ConvertParamAgeToDouble((DateTime)loanApplication.PersonalData.DateOfBirth)
                            : 0))/100;
            }
            catch
            {
                throw new Exception("Not Creditworthy");
            }
        }



        /// <summary>
        /// Calculate amount of points by current Cash income
        /// </summary>
        /// <param name="middleIncome"></param>
        /// <returns></returns>
        private static double ConvertParamCashIncomeLevelToDouble(decimal middleIncome)
        {
            if (middleIncome < 2000000)
            {
                throw new Exception("Not creditworthy");
            }
            return (double) (middleIncome*(decimal) W_MIDDLE_INCOME_MAX_VALUE/10000000);
        }

        private static double ConvertParamChildrenCountToDouble(int childrenCount)
        {
            if (childrenCount == 0)
            {
                return W_CHILDREN_COUNT_ZERO;
            }
            if (childrenCount == 1)
            {
                return W_CHILDREN_COUNT_ONE;
            }
            if (childrenCount == 2)
            {
                return W_CHILDREN_COUNT_TWO;
            }
            if (childrenCount == 3)
            {
                return W_CHILDREN_COUNT_THREE;
            }
            if (childrenCount >= 4)
            {
                return W_CHILDREN_COUNT_MORE_THREE;
            }
            return -1;
        }

        private static double ConvertParamHigherEducationToDouble(Education higherEducation)
        {
            switch (higherEducation)
            {
                case Education.MoreHigher:
                    {
                        return W_EDUCATION_MORE_HIGHER;
                    }
                case Education.Higher:
                    {
                        return W_EDUCATION_HIGHER;
                    }
                case Education.HigherIncomplete:
                    {
                        return W_EDUCATION_HIGHER_INCOMPLETE;
                    }
                case Education.Vocational:
                    {
                        return W_EDUCATION_VOCATIONAL;
                    }
                case Education.Secondary:
                    {
                        return W_EDUCATION_SECONDARY;
                    }
                case Education.LowerSecondary:
                    {
                        return W_EDUCATION_LOWER_SECONDARY;
                    }
            }
            return -1;
        }

        private static double ConvertParamIsMaridToDouble(MaritalStatus isMarried)
        {
            return isMarried == MaritalStatus.Married ? W_IS_MARRIED : W_IS_NOT_MARRIED;
        }

        private static double ConvertParamLenghtOfWorkToDouble(int lenghtOfWork)
        {
            if (lenghtOfWork < 1)
            {
                return W_LENGTH_OF_WORK_LESS_ONE_YEAR;
            }
            if (lenghtOfWork >= 1 && lenghtOfWork < 2)
            {
                return W_LENGTH_OF_WORK_BETWEEN_ONE_TWO_YEARS;
            }
            if (lenghtOfWork >= 2 && lenghtOfWork < 3)
            {
                return W_LENGTH_OF_WORK_BETWEEN_TWO_THREE_YEARS;
            }
            if (lenghtOfWork >= 3 && lenghtOfWork < 4)
            {
                return W_LENGTH_OF_WORK_BETWEEN_THREE_FOUR_YEARS;
            }
            if (lenghtOfWork >= 4 && lenghtOfWork < 5)
            {
                return W_LENGTH_OF_WORK_BETWEEN_FOUR_FIVE_YEARS;
            }
            if (lenghtOfWork >= 5)
            {
                return W_LENGTH_OF_WORK_MORE_FIVE_YEARS;
            }
            return -1;
        }

        private static double ConvertParamIsHomeowner(bool isHomeOwner)
        {
            return isHomeOwner ? W_IS_HOMEOWNER : W_IS_NOT_HOMEOWNER;
        }

        private static double ConvertParamAgeToDouble(DateTime bithday)
        {
            var age = Convert.ToInt32((DateTime.Now - bithday).TotalDays / 365);
            if (age <= 0)
            {
                throw new Exception("age <= 0");
            }
            if (age < 20)
            {
                throw new Exception("Not creditworthy");
            }
            if (20 <= age && age < 25)
            {
                return W_AGE_BETWEEN_20_25;
            }
            if (25 <= age && age <= 60)
            {
                return W_AGE_BETWEEN_25_60;
            }
            if (age > 60 && age < 90)
            {
                return W_AGE_MORE_60;
            }
            throw new Exception("Not creditworthy");
        }
    }
}
