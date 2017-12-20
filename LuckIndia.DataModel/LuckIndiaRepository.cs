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

        public void Dispose()
        {
            Dispose();
        }

        /********************************* Post ****************************************/
        // POST api/users
        public void InsertUser(LuckUser user)
        {
            List<Account> addedaccts = user.accounts.ToList();
            addedaccts.ForEach(x => _context.AcctTypes.AddOrUpdate(x.Type));
            _context.SaveChanges();

            addedaccts.ForEach(x => _context.Accounts.AddOrUpdate(x));
            _context.SaveChanges();

            addedaccts.ForEach(d => user.accounts.Add(d));
            _context.Users.AddOrUpdate(user);
            _context.SaveChanges();
            
        }

    }
}
