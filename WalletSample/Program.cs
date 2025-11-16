using WalletSample.Services;

var walletService = new WalletService();

var wallet1 = Guid.NewGuid();
var wallet2 = Guid.NewGuid();

walletService.CreateWallet(wallet1, 1000);
walletService.CreateWallet(wallet2, 500);

// انتقال موفق
bool result1 = walletService.Transfer(wallet1, wallet2, 200);
Console.WriteLine($"Transfer 1: {result1}");

// انتقال ناموفق (موجودی ناکافی)
bool result2 = walletService.Transfer(wallet1, wallet2, 1000);
Console.WriteLine($"Transfer 2: {result2}");

// نمایش لاگ تراکنش‌ها
var logs = walletService.GetTransactionLogs();
foreach (var log in logs)
{
    Console.WriteLine($"Tx {log.TransactionId}: {log.FromWalletId} -> {log.ToWalletId} | " +
                      $"Amount: {log.Amount} | Success: {log.Success} | Reason: {log.FailureReason}");
}


Console.ReadLine(); 