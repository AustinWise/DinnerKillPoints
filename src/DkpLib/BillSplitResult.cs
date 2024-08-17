namespace Austin.DkpLib;

public record BillSplitResult(string Name, DateTime Date, List<SplitTransaction> Transactions)
{
}
