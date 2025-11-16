using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WalletSample.Entities
{
    public class Wallet
    {
        public Guid WalletId { get; set; }
        public decimal Balance { get; set; }
    }
}
