using LuckIndia.APIs.DAL.Exceptions;
using LuckIndia.Models;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using LuckIndia.DataModel.DAL.Attributes;
using LuckIndia.DataModel.DAL.CachingServices;
using LuckIndia.DataModel.Authorizations;
using LuckIndia.DataModel.Interfaces;
using LuckIndia.DataModel.DAL.ValidationServices.ExpirableValidationServices;
using LuckIndia.DataModel.DAL.CrudHelpers;
using Alphaeon.Services.EnterpriseAPI.DAL.CrudHelpers;
using LuckIndia.DataModel.LoggingServices;
using LuckIndia.Models.Attributes;

namespace LuckIndia.DataModel
{
    public class LuckIndiaDBContext : DbContext
    {

        private readonly ISecurityContext _securityContext;
        private readonly AccessTokenCacheModel _accessTokenCacheModel;
        private int commandTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["CommandTimeOut"]);

        public LuckIndiaDBContext()
        {
            this.Database.CommandTimeout = commandTimeOut;
        }

        private LuckIndiaDBContext(ILogger logger = null)
        {
            //this.Database.CommandTimeout = commandTimeOut;
            this.Logger = logger ?? new FakeLoggerService();
        }

        private LuckIndiaDBContext(string accessToken, ILogger logger = null)
            : this(logger)
        {

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new InvalidAccessTokenException();
            }

            this.Database.CommandTimeout = commandTimeOut;
            //this.Logger.AppendLine("Context(accessToken) Started");
            var sw = Stopwatch.StartNew();

            _securityContext = GetAuthorizedAccessToken(accessToken);

            sw.Stop();
            this.Logger.AppendLine(string.Format("Context(accessToken) Done: {0} ms", sw.ElapsedMilliseconds));
        }

        private LuckIndiaDBContext(ISecurityContext accessToken, ILogger logger = null)
         : this(logger)
        {
            if (accessToken == null)
            {
                throw new InvalidAccessTokenException();
            }

            this.Database.CommandTimeout = commandTimeOut;
            this.Logger.AppendLine("Context(accessToken) Started");
            var sw = Stopwatch.StartNew();

            _securityContext = accessToken;

            sw.Stop();
            this.Logger.AppendLine(string.Format("Context(accessToken) Done: {0} ms", sw.ElapsedMilliseconds));
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public void Detach(object entity)
        {
            ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }

        public Type GetEntityType(object entity)
        {
            return ObjectContext.GetObjectType(entity.GetType());
        }

        public ILogger Logger { get; private set; }


        #region Collection Declarations

        public DbSet<AccessToken> AccessTokens { get; set; }
        //public DbSet<ApplicationAttribute> ApplicationAttributes { get; set; }
        //public DbSet<Application> Applications { get; set; }
        //public DbSet<ApplicationAccessRole> ApplicationAccessRoles { get; set; }
        //public DbSet<ApplicationCollectionFilter> ApplicationCollectionFilters { get; set; }
        //public DbSet<ApplicationCrudHook> ApplicationCrudHooks { get; set; }
        //public DbSet<ApplicationPermission> ApplicationPermissions { get; set; }
        //public DbSet<ApplicationRemoteProcedureCallsPermission> ApplicationRemoteProcedureCallsPermissions { get; set; }
        //public DbSet<RoleRemoteProcedureCallsPermission> RoleRemoteProcedureCallsPermissions { get; set; }
        //public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        //public DbSet<ApplicationToken> ApplicationTokens { get; set; }
        //public DbSet<AccountRole> AccountRoles { get; set; }
        //public DbSet<UserRole> UserRoles { get; set; }
        //public DbSet<Account> Accounts { get; set; }
        //public DbSet<CollectionFilter> CollectionFilters { get; set; }
        //public DbSet<CrudHook> CrudHooks { get; set; }
        //public DbSet<MessageType> MessageTypes { get; set; }
        public DbSet<ModelClass> ModelClasses { get; set; }
        //public DbSet<ModelClassDocument> ModelClassDocuments { get; set; }
        //public DbSet<ModelEvent> ModelEvents { get; set; }
        //public DbSet<ModelEventType> ModelEventTypes { get; set; }
        //public DbSet<RemoteProcedureCall> RemoteProcedureCalls { get; set; }
        //public DbSet<RemoteProcedureCallDocument> RemoteProcedureCallDocuments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<RoleType> RoleTypes { get; set; }
        //public DbSet<User> Users { get; set; }
        //public DbSet<UserFile> UserFiles { get; set; }
        //public DbSet<UserToken> UserTokens { get; set; }
        //public DbSet<Distributor> Distributors { get; set; }
        #endregion


        #region LuckIndia Entities Context

        public DbSet<LuckIndia.Models.User> Users { get; set; }

       public DbSet<LuckUser> LuckUsers { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Bids> Bid { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Quiz> Quizes { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<TxnType> TxnTypes { get; set; }
        public DbSet<AccountType> AcctTypes { get; set; }

        #endregion
        /// <summary>
        /// Gets a CurrentUser object based on the AccessToken from the constructor.
        /// Calling CurrentUser.Id will not trigger a database call, but any role checking will.
        /// </summary>
        public IUser GetCurrentUser()
        {
            if (_securityContext == null)
            {
                return null;
            }
            return _securityContext.User;
        }

        //public int GetCurrentApplicationId()
        //{
        //    if (_securityContext == null)
        //    {
        //        throw new InvalidAccessTokenException();
        //    }

        //    return _securityContext.ApplicationInfo.Id;
        //}

        internal ISecurityContext SecurityContext
        {
            get
            {
                return _securityContext;
            }
        }



        /// <summary>
        /// Validates that the given application token is active.
        /// </summary>
        /// <param name="token"></param>
        //public void ValidateApplicationToken(string token)
        //{
        //    this.Logger.AppendLine("Checking valid application token.");

        //    var applicationToken = this.ApplicationTokens.FirstOrDefault(x => x.Token == token);
        //    if (applicationToken == null || ExpirableValidator.IsExpired(applicationToken.EndDate))
        //    {
        //        throw new InvalidDataException("Invalid ApplicationToken.");
        //    }
        //}

        /// <summary>
        /// Validates that the given application token is active and it has the role of an authority.
        /// </summary>
        /// <param name="token"></param>
        //public void ValidateAuthorityApplicationToken(string token)
        //{
        //    this.Logger.AppendLine("Checking valid authority application token.");

        //    var applicationToken = this.ApplicationTokens.FirstOrDefault(x => x.Token == token);
        //    if (applicationToken == null || ExpirableValidator.IsExpired(applicationToken.EndDate))
        //    {
        //        throw new InvalidDataException("Invalid authority application.");
        //    }

        //    if (!applicationToken.Application.ApplicationRoles.Any(x => x.RoleId == (int)ApplicationRoleValue.AuthenticationAuthority))
        //    {
        //        throw new InvalidDataException("Invalid authority application.");
        //    }
        //}

        /// <summary>
        /// Validates that the given user token is active.
        /// </summary>
        /// <param name="token"></param>
        //public void ValidateUserToken(string token)
        //{
        //    this.Logger.AppendLine("Checking valid UserToken.");

        //    var userToken = this.UserTokens.FirstOrDefault(x => x.Token == token);
        //    if (userToken == null || ExpirableValidator.IsExpired(userToken.EndDate))
        //    {
        //        throw new InvalidDataException("Invalid UserToken.");
        //    }
        //}

        /// <summary>
        /// Validates that the given accessToken is active.
        /// </summary>
        /// <param name="token"></param>
        public void ValidateAccessToken(string token)
        {
            this.Logger.AppendLine("Checking valid access token.");

            var accessToken = this.AccessTokens.FirstOrDefault(x => x.Token == token);
            if (accessToken == null || ExpirableValidator.IsExpired(accessToken.EndDate))
            {
                throw new InvalidAccessTokenException();
            }
        }





        /// <summary>
        /// Gets a single model under the context of the current user.
        /// </summary>
        /// <typeparam name="T">Type of Model</typeparam>
        /// <param name="id">Id of the Model</param>
        public T GetSingle<T>(int id) where T : Model
        {
            var logName = string.Format("for {0}.Id {1}", typeof(T).Name, id);
            this.Logger.AppendLine(string.Format("GetSingle started {0}", logName));

            if (_securityContext == null)
            {
                throw new InvalidAccessTokenException();
            }

            var sw = Stopwatch.StartNew();

            var readHelper = new ReadHelper<T>(id, this);
            readHelper.Execute();

            var model = readHelper.GetModel();

            sw.Stop();
            this.Logger.AppendLine(string.Format("GetSingle done {0}: {1} ms", logName, sw.ElapsedMilliseconds));

            return model;
        }

        /// <summary>
        /// Gets a collection of models under the context of the current user.
        /// This will apply any collection filters.
        /// </summary>
        /// <typeparam name="T">Type of Model</typeparam>
        public IQueryable<T> GetCollection<T>() where T : Model
        {
            var logName = string.Format("for {0}", typeof(T).Name);
            this.Logger.AppendLine(string.Format("GetCollection started {0}", logName));

            if (_securityContext == null)
            {
                throw new InvalidAccessTokenException();
            }

            var sw = Stopwatch.StartNew();

            var type = typeof(T);

            var collectionHelper = new CollectionHelper<T>(this);

            var collection = collectionHelper.GetCollection();

            var includes = this.GetIncludes(type, string.Empty);

            var finalCollection = includes.Aggregate(collection, (current, name) => current.Include(name));

            sw.Stop();
            this.Logger.AppendLine(string.Format("GetCollection done {0}: {1} ms", logName, sw.ElapsedMilliseconds));

            return finalCollection;
        }

        /// <summary>
        /// Uses reflection to look at the Model and find any properties
        /// with the ManToManyObject attribute and then adds it to the
        /// list of includes so EF can pull the entire object with navigation properties from the database.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseName"></param>
        private IEnumerable<string> GetIncludes(Type type, string baseName)
        {
            var list = new List<string>();
            if (!(type.IsDefined(typeof(IncludeAttribute), false)))
            {
                return list;
            }

            var properties = type.GetProperties().Where(x => x.IsDefined(typeof(IncludeAttribute), false));

            foreach (var property in properties)
            {
                var fullName = string.Concat(baseName, property.Name);
                list.Add(fullName);

                var nextBaseName = string.Concat(fullName, ".");

                list.AddRange(this.GetIncludes(property.PropertyType, nextBaseName));
            }

            return list;
        }

        /// <summary>
        /// Updates the given model with the given dictionary of changes.
        /// </summary>
        /// <typeparam name="T">Type of Model</typeparam>
        /// <param name="id">Id of the Model</param>
        /// <param name="delta">Dictionary of changes. The key must match the property name to be changed.</param>
        /// <param name="validatePermissions">Validates that the current user and current application have access to do this. Hooks and rules will typically have this as false.</param>
        public T Update<T>(int id, IDictionary<string, object> delta, bool validatePermissions = true) where T : Model
        {
            var logName = string.Format("for {0}.Id {1}", typeof(T).Name, id);
            this.Logger.AppendLine(string.Format("Update started {0}", logName));

            if (validatePermissions && _securityContext == null)
            {
                throw new InvalidAccessTokenException();
            }

            var sw = Stopwatch.StartNew();

            var model = this.Set<T>().FirstOrDefault(x => x.Id == id);
            if (model == null)
            {
                throw new ModelNotFoundException();
            }

            var updateHelper = new UpdateHelper<T>(model, delta, this, validatePermissions);
            updateHelper.Execute();

            sw.Stop();
            this.Logger.AppendLine(string.Format("Update done {0}: {1} ms", logName, sw.ElapsedMilliseconds));

            //wanted to call GetSingle here to refresh the model, but if permissions are set to not read, that's a problem.
            //If you can update this guy, surely you can read him back, so attach all the includes here

            return this.GetModel<T>(model.Id);
        }

        /// <summary>
        /// Creates the given model.
        /// </summary>
        /// <typeparam name="T">Type of Model</typeparam>
        /// <param name="model">Model to be added. Do not nest models, this should be one model at a time.</param>
        /// <param name="validatePermissions">Validates that the current user and current application have access to do this. Hooks and rules will typically have this as false.</param>
        /// <returns></returns>
        public T Create<T>(T model, bool validatePermissions = true) where T : Model
        {
            var logName = string.Format("for {0}", typeof(T).Name);
            this.Logger.AppendLine(string.Format("Create started {0}", logName));

            if (validatePermissions && _securityContext == null)
            {
                throw new InvalidAccessTokenException();
            }

            var sw = Stopwatch.StartNew();

            var createHelper = new CreateHelper<T>(model, this, validatePermissions);
            createHelper.Execute();

            sw.Stop();
            this.Logger.AppendLine(string.Format("Create done {0}: {1} ms", logName, sw.ElapsedMilliseconds));

            //wanted to call GetSingle here to refresh the model, but if permissions are set to not read, that's a problem.
            //If you can create this guy, surely you can read him back, so attach all the includes here

            return this.GetModel<T>(model.Id);
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <typeparam name="T">Type of Model</typeparam>
        /// <param name="id">ID of the Model</param>
        /// <param name="validatePermissions">Validates that the current user and current application have access to do this. Hooks and rules will typically have this as false.</param>
        public void Delete<T>(int id, bool validatePermissions = true) where T : Model
        {
            var logName = string.Format("for {0}.Id {1}", typeof(T).Name, id);
            this.Logger.AppendLine(string.Format("Delete started {0}", logName));

            if (validatePermissions && _securityContext == null)
            {
                throw new InvalidAccessTokenException();
            }

            var sw = Stopwatch.StartNew();

            var model = this.Set<T>().FirstOrDefault(x => x.Id == id);
            if (model == null)
            {
                throw new ModelNotFoundException();
            }

            var deleteHelper = new DeleteHelper<T>(model, this, validatePermissions);
            deleteHelper.Execute();

            sw.Stop();
            this.Logger.AppendLine(string.Format("Delete done {0}: {1} ms", logName, sw.ElapsedMilliseconds));
        }

        /// <summary>
        /// Gets the given model with all the ManyToMany navigation properties set.
        /// This does not check any permissions and should only be called from the Update and Create methods.
        /// </summary>
        /// <typeparam name="T">Type of Model</typeparam>
        /// <param name="id">Id of the Model</param>
        private T GetModel<T>(int id) where T : Model
        {
            var logName = string.Format("for {0}.{1}", typeof(T).Name, id);
            this.Logger.AppendLine(string.Format("GetModel started {0}", logName));
            var sw = Stopwatch.StartNew();

            var type = typeof(T);

            var includes = this.GetIncludes(type, string.Empty);

            var collection = this.Set<T>().Where(x => x.Id == id);

            var model = includes.Aggregate(collection, (current, name) => current.Include(name)).FirstOrDefault();

            sw.Stop();
            this.Logger.AppendLine(string.Format("GetModel done {0}: {1} ms", logName, sw.ElapsedMilliseconds));

            return model;
        }




        /// <summary>
        /// Executes a raw SQL runnable.
        /// </summary>
        /// <param name="runnable"></param>
        //public void ExecuteSqlRunnable(ISqlRunnable runnable)
        //{
        //    var sql = runnable.GetSqlCommand();
        //    if (null == sql || string.IsNullOrEmpty(sql.CommandText))
        //    {
        //        return;
        //    }

        //    this.Logger.AppendLine(string.Format("{0} Started", runnable));

        //    var parameters = new object[sql.Parameters.Count];
        //    sql.Parameters.CopyTo(parameters, 0);
        //    sql.Parameters.Clear();

        //    this.Logger.AppendLine(string.Format("SQL: {0}", sql.CommandText));
        //    foreach (var paramString in parameters.Select(param => param.ToString()))
        //    {
        //        this.Logger.AppendLine(string.Format("SQL Parameter: {0}", paramString));
        //    }

        //    var sw = Stopwatch.StartNew();

        //    this.Database.ExecuteSqlCommand(sql.CommandText, parameters);

        //    sw.Stop();

        //    this.Logger.AppendLine(string.Format("{0} Done: {1} ms", runnable, sw.ElapsedMilliseconds));
        //}

        /// <summary>
        /// Executes the given runnable and returns an object of the given type.
        /// </summary>
        /// <typeparam name="T">Type of object to return.</typeparam>
        /// <param name="runnable">Runnable to execute.</param>
        //public T ExecuteRunnable<T>(IRunnable runnable) where T : class
        //{
        //    this.Logger.AppendLine(string.Format("{0} Started", runnable));

        //    var sw = Stopwatch.StartNew();

        //    var t = runnable.Execute<T>(this);

        //    sw.Stop();

        //    this.Logger.AppendLine(string.Format("{0} Done: {1} ms", runnable, sw.ElapsedMilliseconds));

        //    return t;
        //}

        /// <summary>
        /// Executes the given runnable.
        /// </summary>
        /// <param name="runnable">Runnable to execute.</param>
        //public void ExecuteRunnable(IRunnable runnable)
        //{
        //    this.Logger.AppendLine(string.Format("{0} Started", runnable));

        //    var sw = Stopwatch.StartNew();

        //    runnable.Execute(this);

        //    sw.Stop();

        //    this.Logger.AppendLine(string.Format("{0} Done: {1} ms", runnable, sw.ElapsedMilliseconds));
        //}

        private ISecurityContext GetAuthorizedAccessToken(string accessToken)
        {
            ISecurityContext retVal = null;
            var authAccessToken = AuthProviderFactory.GetAuthProvider().Verify(accessToken);//AccessTokenAuthorization.getAuthorizedAccessToken(accessToken, out status);
            retVal = authAccessToken;
            return retVal;
        }

        /// <summary>
        /// Gets a DatabaseContext that does not validate an AccessToken.
        /// This is usefull for executing runnables that do not need to run in scope of a user.
        /// Any CRUD operations will throw InvalidAccessToken exceptions.
        /// </summary>
        /// <param name="logger">Option logger.</param>
        public static LuckIndiaDBContext GetContext(ILogger logger = null)
        {

            return new LuckIndiaDBContext(logger);
        }

        /// <summary>
        /// Gets a DatabaseContext that does not validate an AccessToken.
        /// This is usefull for executing runnables that do not need to run in scope of a user.
        /// Any CRUD operations will throw InvalidAccessToken exceptions.
        /// </summary>
        /// <param name="logger">Option logger.</param>
        //public static LuckIndiaDBContext GetContextWithAccessToken(string accessToken, ILogger logger = null)
        //{
        //    return new LuckIndiaDBContext(accessToken, logger);
        //}

        public static LuckIndiaDBContext GetContextWithAccessToken(SecurityContext accessToken, ILogger logger = null)
        {
            return new LuckIndiaDBContext(accessToken, logger);
        }
    }
    }
 