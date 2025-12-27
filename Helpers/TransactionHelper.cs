namespace WalletAPI.Helpers
{
    public static class TransactionHelper
    {
        public static string GenerateTxnRefNo(string prefix = "TXN")
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"{prefix}{timestamp}{random}";
        }

        public static string GenerateTopupRefNo()
        {
            return GenerateTxnRefNo("TOP");
        }

        public static string GenerateDebitRefNo()
        {
            return GenerateTxnRefNo("DBT");
        }
    }
}
