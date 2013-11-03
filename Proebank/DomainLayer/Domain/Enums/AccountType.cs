﻿namespace Domain.Enums
{
    public enum AccountType : int
    {
        // счёт баланса банка, т.е. сколько у него денег. Цифры взяты с потолка
        BankBalance = 1000,
        // счёт обслуживания контракта
        ContractService = 3819,
        // счёт учёта основного долга
        GeneralDebt = 2412,
        // счёт учёта процентов
        Interest = 2471, // + комиссия 6701, но в нашем проекте можно её не учитывать - сейчас её официально нет
        // счёт учёта просроченного основного долга
        OverdueGeneralDebt = 2481,
        // счёт учёта просроченных процентов
        OverdueInterest = 2491,
    }
}