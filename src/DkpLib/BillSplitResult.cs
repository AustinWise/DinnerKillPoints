namespace Austin.DkpLib;

public class BillSplitResult
{
    public string Name { get; set; }
    public List<SplitTransaction> Transactions { get; set; }
}
