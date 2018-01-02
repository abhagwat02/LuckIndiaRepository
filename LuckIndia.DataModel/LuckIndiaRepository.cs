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
            return bIncludeAccounts ?_context.LuckUsers.Include("accounts.type").ToList(): _context.LuckUsers.ToList();
        }

        #region Get For User & Account
        public LuckUser GetUser(int Id, bool bIncludeAccounts = true)
        {
            return bIncludeAccounts ? 
                _context.LuckUsers.Include("accounts.type")
                .Where(x=> x.Id == Id)
                .FirstOrDefault() : _context.LuckUsers
                                    .Where(x=> x.Id == Id)
                                     .FirstOrDefault();
        }

        public LuckUser GetUserByPhone(long phonenumber, bool bIncludeAccounts = true)
        {
            return bIncludeAccounts ?
                _context.LuckUsers.Include("accounts.type")
                .Where(x => x.PhoeNumber == phonenumber)
                .FirstOrDefault() : _context.LuckUsers.Where(x => x.PhoeNumber == phonenumber)
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

        public Account GetAccountByUserName(String username)
        {
            return _context.Accounts
                .Include("type")
                .Include("user")
                .Where(x => x.UserName.CompareTo(username) == 0).FirstOrDefault();
        }
        public AccountType GetAccountType(int Id)
        {
            return _context.AcctTypes.Where(x => x.Id == Id).FirstOrDefault();
        }

        #endregion

        #region Get for Quiz
        public List<Question> GetAllQuestion(bool bIncludeOption)
        {
            return bIncludeOption ? _context.Questions.Include("options").ToList() : _context.Questions.ToList();

        }

        public List<Option> GetAllOptionForQuestion(int QuesID)
        {
            return  _context.Options.Where(x=> x.Question.Id == QuesID).ToList();

        }
        #endregion

        /********************************* Post ****************************************/
        #region Create for User and Account
        // POST api/users
        public void CreateUser(LuckUser user)
        {
            var now = DateTime.Now;
            user.DateCreated = new DateTime(now.Year, now.Month, now.Day,
                                    now.Hour, now.Minute, now.Second);

            user.DateModified = new DateTime(now.Year, now.Month, now.Day,
                                    now.Hour, now.Minute, now.Second);
            _context.LuckUsers.AddOrUpdate(user);
            _context.SaveChanges();
            
        }

        public void CreateAccount(Account acct)
        {
            try
            {
                var now = DateTime.Now;
                acct.DateCreated = new DateTime(now.Year, now.Month, now.Day,
                                        now.Hour, now.Minute, now.Second);

                acct.DateModified = new DateTime(now.Year, now.Month, now.Day,
                                    now.Hour, now.Minute, now.Second);

                _context.Entry(acct.user).State = System.Data.Entity.EntityState.Unchanged;
                acct.Type = _context.AcctTypes.Where(x=> x.TypeName == acct.Type.TypeName).FirstOrDefault();
                CreateAccountType(acct.Type);
                _context.Accounts.AddOrUpdate(acct);
                _context.SaveChanges();
            }

            catch (Exception ex)
            {            }
        }

        public void CreateAccountType(AccountType accttype)
        {
            var now = DateTime.Now;
            accttype.DateCreated = new DateTime(now.Year, now.Month, now.Day,
                                    now.Hour, now.Minute, now.Second);

            accttype.DateModified = new DateTime(now.Year, now.Month, now.Day,
                                    now.Hour, now.Minute, now.Second);

            if (_context.AcctTypes.Any(x=> x.Id == accttype.Id))
                _context.Entry(accttype).State = System.Data.Entity.EntityState.Unchanged;

            _context.AcctTypes.AddOrUpdate(accttype);
            _context.SaveChanges();

        }

        #endregion

        #region Create for Quiz
        public void CreateQuestion(Question ques)
        {
            var now = DateTime.Now;
            ques.DateCreated = new DateTime(now.Year, now.Month, now.Day,
                                    now.Hour, now.Minute, now.Second);

            ques.DateModified = new DateTime(now.Year, now.Month, now.Day,
                                    now.Hour, now.Minute, now.Second);
            _context.Questions.AddOrUpdate(ques);
            _context.SaveChanges();

        }

        public void CreateOption(Option option)
        {
            var now = DateTime.Now;
            option.DateCreated = new DateTime(now.Year, now.Month, now.Day,
                                    now.Hour, now.Minute, now.Second);

            option.DateModified = new DateTime(now.Year, now.Month, now.Day,
                                    now.Hour, now.Minute, now.Second);

            _context.Entry(option.Question).State = System.Data.Entity.EntityState.Unchanged;
            _context.Options.AddOrUpdate(option);
            _context.SaveChanges();
            
        }
        #endregion

        /***************************************** Delete *************************************************/
        #region Delete for User and Account
        public bool DeleteUser(int Id)
        {

            var accountsForUser = GetAccountsForUser(Id);
            accountsForUser.ForEach(x => DeleteAccount(x.Id));

            LuckUser deleteUser = GetUser(Id);
            var deletedUser = _context.LuckUsers.Remove(deleteUser);
            _context.SaveChanges();

            return deletedUser != null ?  true :  false;
        }

        public bool DeleteAccount(int Id)
        {


            Account deleteAcct = GetAccount(Id);
            var deletedUser = _context.Accounts.Remove(deleteAcct);
            _context.SaveChanges();

            return deleteAcct != null ? true : false;
        }
        #endregion


        /*************************************** Patch ****************************************************/
        #region Update for User and Account
        public LuckUser UpdateUser(int Id, LuckUser user)
        {
            var now = DateTime.Now;
            user.DateModified = new DateTime(now.Year, now.Month, now.Day,
                                    now.Hour, now.Minute, now.Second);
            var userTobeUpdated = GetUser(Id);
            if (userTobeUpdated != null)
            {
                userTobeUpdated.FirstName = user.FirstName;
                userTobeUpdated.MiddleName = user.MiddleName;
                userTobeUpdated.LastName = user.LastName;
                userTobeUpdated.Address = user.Address;
                userTobeUpdated.PhoeNumber = user.PhoeNumber;
                
                 user.accounts.ToList().ForEach(x=> UpdateAccount(x.Id, x));
                _context.SaveChanges();

                return userTobeUpdated;

            }
            return null;
        }
        public Account UpdateAccount(int Id, Account acct)
        {
            var now = DateTime.Now;
            acct.DateModified = new DateTime(now.Year, now.Month, now.Day,
                                    now.Hour, now.Minute, now.Second);
           

            var acctTobeUpdated = GetAccount(Id);
            if (acctTobeUpdated != null)
            { 
                acctTobeUpdated.CardNumber = acct.CardNumber;
                acctTobeUpdated.DateModified = acct.DateModified;
                acctTobeUpdated.ParentAccountID = acct.ParentAccountID;
                acctTobeUpdated.Password = acct.Password;
                acctTobeUpdated.UserName = acct.UserName;
                acctTobeUpdated.Type = GetAccountType(acct.Type.Id);
                _context.SaveChanges();
                return acctTobeUpdated;

            }
            return null;
        }
        #endregion

      

        public void Dispose()
        {
            Dispose();
        }

    }
}
