namespace Austin.DkpLib;

public record BillSplitResult(string Name, List<SplitTransaction> Transactions)
{
}
