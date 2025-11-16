using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WalletSample.Entities;

namespace WalletSample.Services
{

    public class WalletService
    {
        private readonly ConcurrentDictionary<Guid, Wallet> _wallets;
        private readonly ConcurrentDictionary<Guid, object> _walletLocks;
        private readonly List<Transaction> _transactionLogs;
        private readonly object _logLock = new object();

        public WalletService()
        {
            _wallets = new ConcurrentDictionary<Guid, Wallet>();
            _walletLocks = new ConcurrentDictionary<Guid, object>();
            _transactionLogs = new List<Transaction>();
        }

        public void CreateWallet(Guid walletId, decimal initialBalance = 0)
        {
            _wallets[walletId] = new Wallet { WalletId = walletId, Balance = initialBalance };
            _walletLocks[walletId] = new object();
        }

        /// <summary>
        /// در محیط واقعی محاسبه موجودی کیف پول بهتر است از جدول تراکنش های ثبت شده انجام شود 
        /// و از فیلد مشخص جهت ثبت موجودی دوری شود تا اطمینان حاصل شود
        ///  فقط تراکنش های تکمیل شده روی موجودی اثر دارند
        /// </summary>
        /// <param name="fromWalletId"></param>
        /// <param name="toWalletId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public bool Transfer(Guid fromWalletId, Guid toWalletId, decimal amount)
        {
            var transactionId = Guid.NewGuid();

            if (!_wallets.ContainsKey(fromWalletId) || !_wallets.ContainsKey(toWalletId))
            {
                LogTransaction(transactionId, fromWalletId, toWalletId, amount, false, "کیف پول مبدا یا مقصد پیدا نشد!");
                return false;
            }

            if (amount <= 0)
            {
                LogTransaction(transactionId, fromWalletId, toWalletId, amount, false, "مقدار اشتباه");
                return false;
            }

            var lock1 = fromWalletId.CompareTo(toWalletId) < 0 ? fromWalletId : toWalletId;
            var lock2 = fromWalletId.CompareTo(toWalletId) < 0 ? toWalletId : fromWalletId;

            lock (_walletLocks[lock1])
                lock (_walletLocks[lock2])
                {
                    var fromWallet = _wallets[fromWalletId];
                    var toWallet = _wallets[toWalletId];

                    if (fromWallet.Balance < amount)
                    {
                        LogTransaction(transactionId, fromWalletId, toWalletId, amount, false, "موجودی ناکافی");
                        return false;
                    }

                    try
                    {
                        fromWallet.Balance -= amount;
                        toWallet.Balance += amount;

                        LogTransaction(transactionId, fromWalletId, toWalletId, amount, true, string.Empty);
                        return true;
                    }
                    catch(Exception ex)
                    {
                        //در محیط واقعی در این قسمت باید Rollback شود تا تمامی رویدادها به حالت قبل برگردد
                       
                        LogTransaction(transactionId, fromWalletId, toWalletId, amount, false, $"{ex.Message} خطای پیش بینی نشده ");
                        
                        return false;
                    }
                    //finally
                    //{

                    //}
                }
        }

        private void LogTransaction(Guid transactionId, Guid fromWalletId, Guid toWalletId,
            decimal amount, bool success, string failureReason)
        {
            lock (_logLock)
            {
                _transactionLogs.Add(new Transaction
                {
                    TransactionId = transactionId,
                    FromWalletId = fromWalletId,
                    ToWalletId = toWalletId,
                    Amount = amount,
                    Timestamp = DateTime.UtcNow,
                    Success = success,
                    FailureReason = failureReason
                });
            }
        }

        public List<Transaction> GetTransactionLogs()
        {
            lock (_logLock)
            {
                return new List<Transaction>(_transactionLogs);
            }
        }
    }
}
