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
        private const double W_MIDDLE_INCOME_BETWEEN_2_AND_3_MLN = 15;
        private const double W_MIDDLE_INCOME_BETWEEN_3_AND_5_MLN = 25;
        private const double W_MIDDLE_INCOME_BETWEEN_5_AND_10_MLN = 40;
        private const double W_MIDDLE_INCOME_MORE_10_MLN = 50;

        //Children Count
        private const double W_CHILDREN_COUNT_ZERO = 0;
        private const double W_CHILDREN_COUNT_ONE = -5;
        private const double W_CHILDREN_COUNT_TWO = -10;
        private const double W_CHILDREN_COUNT_THREE = -15;
        private const double W_CHILDREN_COUNT_MORE_THREE = -20;

        //Education
        private const double W_EDUCATION_MORE_HIGHER = 10;
        private const double W_EDUCATION_HIGHER = 8;
        private const double W_EDUCATION_HIGHER_INCOMPLETE = 5;
        private const double W_EDUCATION_VOCATIONAL = 3;
        private const double W_EDUCATION_SECONDARY = -5;
        private const double W_EDUCATION_LOWER_SECONDARY = -10;

        //Is Married
        private const double W_IS_MARRIED = 10;
        private const double W_IS_NOT_MARRIED = 0;

        //Length of work
        private const double W_LENGTH_OF_WORK_LESS_ONE_YEAR = 5;
        private const double W_LENGTH_OF_WORK_MORE_ONE_YEAR = 10;

        //Is Homeowner
        private const double W_IS_HOMEOWNER = 10;
        private const double W_IS_NOT_HOMEOWNER = -10;

        //Age
        private const double W_AGE_BETWEEN_20_25 = 5;
        private const double W_AGE_BETWEEN_25_60 = 10;
        private const double W_AGE_MORE_60 = 5;




        public static double CalculatePredictions(LoanApplication loanApplication)
        {
            //var sumOfParams = CalculateSumOfParams(loanApplication);
            //return 1.0 / (1.0 + Math.Exp((-1) * sumOfParams));
            
            const double w0 = 0;
            try
            {
                return w0
                       + ConvertParamCashIncomeLevelToDouble(loanApplication.MiddleIncome)
                       + ConvertParamChildrenCountToDouble(loanApplication.ChildrenCount)
                       + ConvertParamHigherEducationToDouble(loanApplication.HigherEducation)
                       + ConvertParamIsMaridToDouble(loanApplication.IsMarried)
                       + ConvertParamLenghtOfWorkToDouble(loanApplication.LengthOfWork)
                       + ConvertParamIsHomeowner(loanApplication.IsHomeowner)
                       +
                       (loanApplication.PersonalData.DateOfBirth != null
                            ? ConvertParamAgeToDouble((DateTime) loanApplication.PersonalData.DateOfBirth)
                            : 0);
            }
            catch
            {
                return 0;
            }
        }

        //private static double CalculateSumOfParams(LoanApplication loanApplication)
        //{
        //    const double w0 = 0;
        //    return w0
        //           + ConvertParamCashIncomeLevelToDouble(loanApplication.MiddleIncome)
        //           + ConvertParamChildrenCountToDouble(loanApplication.ChildrenCount)
        //           + ConvertParamHigherEducationToDouble(loanApplication.HigherEducation)
        //           + ConvertParamIsMaridToDouble(loanApplication.IsMarried)
        //           + ConvertParamLenghtOfWorkToDouble(loanApplication.LengthOfWork)
        //           + ConvertParamIsHomeowner(loanApplication.IsHomeowner)
        //           + (loanApplication.PersonalData.DateOfBirth != null?ConvertParamAgeToDouble((DateTime)loanApplication.PersonalData.DateOfBirth) : 0);
        //}

        private static double ConvertParamCashIncomeLevelToDouble(decimal middleIncome)
        {
            if (middleIncome < 2000000)
            {
                throw new Exception("Not creditworthy");
            }
            if (2000000 <= middleIncome && middleIncome < 3000000)
            {
                return W_MIDDLE_INCOME_BETWEEN_2_AND_3_MLN;
            }
            if (3000000 <= middleIncome && middleIncome < 5000000)
            {
                return W_MIDDLE_INCOME_BETWEEN_3_AND_5_MLN;
            }
            if (5000000 <= middleIncome && middleIncome < 10000000)
            {
                return W_MIDDLE_INCOME_BETWEEN_5_AND_10_MLN;
            }
            if (middleIncome >= 10000000)
            {
                return W_MIDDLE_INCOME_MORE_10_MLN;
            }
            return -1;
        }

        private static double ConvertParamChildrenCountToDouble(int childrenCount)
        {
            if (childrenCount <= 0)
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

            return W_CHILDREN_COUNT_MORE_THREE;
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
            return lenghtOfWork <= 1 ? W_LENGTH_OF_WORK_LESS_ONE_YEAR : W_LENGTH_OF_WORK_MORE_ONE_YEAR;
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
            return W_AGE_MORE_60;
        }
    }
}
