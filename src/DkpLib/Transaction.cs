namespace Austin.DkpLib
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public int DebtorId { get; set; }
        public int CreditorId { get; set; }
        public Money Amount { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public int? BillId { get; set; }

        public virtual BillSplit Bill { get; set; }
        public virtual Person Creditor { get; set; }
        public virtual Person Debtor { get; set; }
    }
}
