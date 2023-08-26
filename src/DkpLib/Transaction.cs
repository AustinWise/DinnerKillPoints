namespace Austin.DkpLib
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public int DebtorId { get; set; }
        public int CreditorId { get; set; }
        public Money Amount { get; set; }
        public DateTime Created { get; set; }
    }
}
