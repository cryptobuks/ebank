﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool
//     Changes to this file will be lost if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

internal class Loan
{
	public virtual LoanApplication Application
	{
		get;
		set;
	}

	public virtual IEnumerable<Account> Accounts
	{
		get;
		set;
	}

	public virtual PaymentSchedule PaymentSchedule
	{
		get;
		set;
	}

}

