using Domain.Models.Accounts;
using Domain.Models.Calendars;
using Domain.Models.Loans;
using Infrastructure.FakeRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    class FakeUnitOfWork : IUnitOfWork
    {
        private AccountRepository _accountRepository;
        private LoanRepository _loanRepository;
        private LoanApplicationRepository _loanApplicationRepository;
        private TariffRepository _tariffRepository;
        private CalendarRepository _calendarRepository;
        private object _isDisposedIndicator;

        public FakeUnitOfWork()
        {
            _isDisposedIndicator = new object();
            _accountRepository = new AccountRepository(_isDisposedIndicator);
            _loanRepository = new LoanRepository(_isDisposedIndicator);
            _loanApplicationRepository = new LoanApplicationRepository(_isDisposedIndicator);
            _tariffRepository = new TariffRepository(_isDisposedIndicator);
            _calendarRepository = new CalendarRepository(_isDisposedIndicator);
        }

        public IAccountRepository AccountRepository
        {
            get { return _accountRepository; }
        }

        public ILoanRepository LoanRepository
        {
            get { return _loanRepository; }
        }

        public ILoanApplicationRepository LoanApplicationRepository
        {
            get { return _loanApplicationRepository; }
        }

        public ITariffRepository TariffRepository
        {
            get { return _tariffRepository; }
        }

        public ICalendarRepository CalendarRepository
        {
            get { return _calendarRepository; }
        }

        public void Save()
        {
            // do nothing here - we already have in-memory model in actual state
        }

        public void Dispose()
        {
            if (_isDisposedIndicator != null)
            {
                _isDisposedIndicator = null;
            }
            else
            {
                throw new ObjectDisposedException("Context is already disposed");
            }
        }
    }
}
