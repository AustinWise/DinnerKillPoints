namespace Austin.DkpLib
{
    public class ApplicationDbContext
    {
        public List<BillSplit> BillSplit => new List<BillSplit>();
        public List<Transaction> Transaction => new List<Transaction>();
        public void SaveChanges() => throw new NotImplementedException();
    }
}
