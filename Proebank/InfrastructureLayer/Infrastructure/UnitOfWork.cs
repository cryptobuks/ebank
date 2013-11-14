using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public UnitOfWork()
        {
            _context = new DataContext();
        }

        public Domain.Models.Accounts.IAccountRepository AccountRepository
        {
            get
            {
                if (this._accountRepository == null)
                {
                    this._accountRepository = new AccountRepository(_context);
                }
                return _accountRepository;
            }
        }

        public Domain.Models.Loans.ILoanRepository LoanRepository
        {
            get
            {
                if (this._loanRepository == null)
                {
                    this._loanRepository = new LoanRepository(_context);
                }
                return _loanRepository;
            }
        }

        public Domain.Models.Loans.ILoanApplicationRepository LoanApplicationRepository
        {
            get
            {
                if (this._loanApplicationRepository == null)
                {
                    this._loanApplicationRepository = new LoanApplicationRepository(_context);
                }
                return _loanApplicationRepository;
            }
        }

        public Domain.Models.Loans.ITariffRepository TariffRepository
        {
            get
            {
                if (this._tariffRepository == null)
                {
                    this._tariffRepository = new TariffRepository(_context);
                }
                return _tariffRepository;
            }
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
