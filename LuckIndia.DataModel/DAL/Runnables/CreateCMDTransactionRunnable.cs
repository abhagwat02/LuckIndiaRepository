using Alphaeon.Services.EnterpriseAPI.Models;
using ALPHAEON.CMD.Common;
using System;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    public class CreateCMDTransactionRunnable : IRunnable
    {
        private readonly int? _cmdUserID;
        private readonly DateTime? _txDate;
        private readonly int? _cmdTxTypeID;
        private readonly string _note;
        private readonly DateTime? _txCompletedDate;
        private readonly int _cmdAccountID;
        private readonly string _city;
        private readonly int? _cmdStateID;
        private readonly bool _isActive;
        private readonly DateTime _createdDate;
        private readonly DateTime _updatedDate;
        private readonly string _updatedBy;
        private readonly Double? _amount;
        private readonly Double? _amountAfterDiscount;
        private readonly string _currencyType;

        public CreateCMDTransactionRunnable(int? cmdUserID, DateTime? txDate, double? amount, double? AmountAfterDiscount, int? cmdTxTypeID, string note, DateTime? txCompletedDate, int cmdAccountID, string city, int? cmdStateID, bool isActive, DateTime createdDate, DateTime updatedDate, string updatedBy, string currencyType)
        {
            _cmdUserID = cmdUserID;
            _txDate = txDate;
            _cmdTxTypeID = cmdTxTypeID;
            _note = note;
            _txCompletedDate = txCompletedDate;
            _cmdAccountID = cmdAccountID;
            _city = city;
            _cmdStateID = cmdStateID;
            _isActive = isActive;
            _createdDate = createdDate;
            _updatedDate = updatedDate;
            _updatedBy = updatedBy;
            _amount = amount;
            _amountAfterDiscount = AmountAfterDiscount;
            _currencyType = currencyType;
        }

        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public T Execute<T>(CMDDatabaseContext context) where T : class
        {
            var transactionLog = new CMDTransaction
            {
                CMDUserID = _cmdUserID,
                TxDate = CommonUtility.GetDate(_txDate),
                CMDTransactionTypeID = _cmdTxTypeID,
                Note = _note,
                TxCompletedDate = CommonUtility.GetDate(_txCompletedDate),
                CMDAccountID = _cmdAccountID,
                City = _city,
                CMDStateID = _cmdStateID,
                CreatedDate = _createdDate,
                UpdatedDate = _updatedDate,
                IsActive = _isActive,
                UpdatedBy = _updatedBy,
                Amount = _amount,
                AmountAfterDiscount = _amountAfterDiscount,
                CurrencyType = _currencyType
            };

            return context.Create(transactionLog, false) as T;
        }

    }
}