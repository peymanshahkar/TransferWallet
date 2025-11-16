using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalletSample.Entities
{
    public class Transaction
    {
        public Guid TransactionId { get; set; }
        public Guid FromWalletId { get; set; }
        public Guid ToWalletId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Success { get; set; }
        public string FailureReason { get; set; }
    }
}
