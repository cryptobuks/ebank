using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Loans;

namespace Application
{
    public class ScoringSystem
    {
        private  const double W_MIDDLE_INCOME = 0.4;
        private  const double W_CHILDREN_COUNT = -0.1;
        private  const double W_HIGHER_EDUCATION = 1;
        private  const double W_IS_MARRIED = 0.3;
        private  const double W_LENGTH_OF_WORK = 2;
        private  const double W_IS_HOME_OWNER = 0.2;
        private  const double W_AGE_IS_BETWEEN_25_50 = 0.25;
        private  const double W_AGE_MORE_50 = -0.1;

        public static double CalculatePredictions(LoanApplication loanApplication)
        {
            var sumOfParams = (-1)*CalculateSumOfParams(loanApplication);
            return 1.0 / (1.0 + Math.Exp(sumOfParams));
        }

        private static double CalculateSumOfParams(LoanApplication loanApplication)
        {
            const double w0 = -95.0;
            return w0;
            //+ ConvertParamCashIncomeLevelToDouble()
            //+ ConvertParamChildrenCountToDouble()
            //+ ConvertParamHigherEducationToDouble()
            //+ ConvertParamIsMaridToDouble()
            //+ ConvertParamLenghtOfWorkToDouble()
            //+ ConvertParamIsHomeowner()
            //+ ConvertParamAgeToDouble();
        }

        private static double ConvertParamCashIncomeLevelToDouble(decimal middleIncome)
        {
            return (double)((decimal)W_MIDDLE_INCOME * middleIncome);
        }

        private static double ConvertParamChildrenCountToDouble(int childrenCount)
        {
            return W_CHILDREN_COUNT * childrenCount;
        }

        private static double ConvertParamHigherEducationToDouble(bool higherEducation)
        {
            return W_HIGHER_EDUCATION*Convert.ToDouble(higherEducation);
        }

        private static double ConvertParamIsMaridToDouble(bool isMarried)
        {
            return W_IS_MARRIED*Convert.ToDouble(isMarried);
        }

        private static double ConvertParamLenghtOfWorkToDouble(int lenghtOfWork)
        {
            return W_LENGTH_OF_WORK*lenghtOfWork;
        }

        private static double ConvertParamIsHomeowner(bool isHomeOwner)
        {
            return W_IS_HOME_OWNER*Convert.ToDouble(isHomeOwner);
        }

        private static double ConvertParamAgeToDouble(DateTime Bithday)
        {
            var age = (DateTime.Now - Bithday).TotalDays/365;
            if (age <= 0)
            {
                throw  new Exception("age <= 0");
            }
            else if (age < 18)
            {
                throw new Exception("age < 18");
            }
            else if (18 <= age && age <= 25)
            {
                throw new Exception("age is between 18 and 25. Too small!!");
            }
            if (25 <= age && age <=50)
            {
                return W_AGE_IS_BETWEEN_25_50*age;
            }
            return W_AGE_MORE_50*age;
        }
    }
}
