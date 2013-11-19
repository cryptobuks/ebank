using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Accounts;
using Domain.Models.Calendars;
using Domain.Models.Loans;
using Infrastructure.Repositories;

namespace Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private AccountRepository _accountRepository;
        private LoanRepository _loanRepository;
        private LoanApplicationRepository _loanApplicationRepository;
        private TariffRepository _tariffRepository;
        private CalendarRepository _calendarRepository;

        public UnitOfWork()
        {
            _context = new DataContext();
        }

        public IAccountRepository AccountRepository
        {
            get
            {
                if (_accountRepository == null)
                {
                    _accountRepository = new AccountRepository(_context);
                }
                return _accountRepository;
            }
        }

        public ILoanRepository LoanRepository
        {
            get
            {
                if (_loanRepository == null)
                {
                    _loanRepository = new LoanRepository(_context);
                }
                return _loanRepository;
            }
        }

        public ILoanApplicationRepository LoanApplicationRepository
        {
            get
            {
                if (_loanApplicationRepository == null)
                {
                    _loanApplicationRepository = new LoanApplicationRepository(_context);
                }
                return _loanApplicationRepository;
            }
        }

        public ITariffRepository TariffRepository
        {
            get
            {
                if (_tariffRepository == null)
                {
                    _tariffRepository = new TariffRepository(_context);
                }
                return _tariffRepository;
            }
        }

        public ICalendarRepository CalendarRepository
        {
            get
            {
                if (_calendarRepository == null)
                {
                    _calendarRepository = new CalendarRepository(_context);
                }
                return _calendarRepository;
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
