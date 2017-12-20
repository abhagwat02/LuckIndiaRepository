using LuckIndia.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckIndia.DataModel
{
    public class LuckIndiaRepository : IDisposable
    {
        private LuckIndiaDBContext _context { get; set; }
        public LuckIndiaRepository(LuckIndiaDBContext context)
        {
            _context = context;
        }
        public List<LuckUser> GetAllUsers(bool bIncludeAccounts = true)
        {
            return bIncludeAccounts ?_context.Users.Include("accounts.type").ToList(): _context.Users.ToList();
        }

        public LuckUser GetUser(int Id, bool bIncludeAccounts = true)
        {
            return bIncludeAccounts ? 
                _context.Users.Include("accounts.type")
                .Where(x=> x.Id == Id)
                .FirstOrDefault() : _context.Users
                                    .Where(x=> x.Id == Id)
                                     .FirstOrDefault();
        }

        public LuckUser GetUserByPhone(long phonenumber, bool bIncludeAccounts = true)
        {
            return bIncludeAccounts ?
                _context.Users.Include("accounts.type")
                .Where(x => x.PhoeNumber == phonenumber)
                .FirstOrDefault() : _context.Users
                                    .Where(x => x.PhoeNumber == phonenumber)
                                     .FirstOrDefault();
        }



        public List<Account> GetAccountsForUser(int userID)
        {
            return _context.Accounts
                .Include("type")
                .Include("user")
                .Where(x => x.user.Id == userID).ToList();
        }

        public Account GetAccount(int Id)
        {
            return _context.Accounts
                .Include("type")
                .Include("user")
                .Where(x => x.Id == Id).FirstOrDefault();
        }

        public AccountType GetAccountType(int Id)
        {
            return _context.AcctTypes.Where(x => x.Id == Id).FirstOrDefault();
        }
        public void Dispose()
        {
            Dispose();
        }

        /********************************* Post ****************************************/
        // POST api/users
        public void CreateUser(LuckUser user)
        {
            _context.Users.AddOrUpdate(user);
            _context.SaveChanges();
            
        }

        public void CreateAccount(Account acct)
        {
            acct.Type = GetAccountType(acct.Type.Id);
            _context.Accounts.AddOrUpdate(acct);
            _context.SaveChanges();
         

        }

        public void CreateAccountType(AccountType accttype)
        {
            _context.AcctTypes.AddOrUpdate(accttype);
            _context.SaveChanges();

        }

    }
}
