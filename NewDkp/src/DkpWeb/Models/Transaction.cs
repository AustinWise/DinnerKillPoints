using System;
using System.Collections.Generic;

namespace DkpWeb.Models
{
    public partial class Transaction
    {
        public Guid Id { get; set; }
        public int DebtorId { get; set; }
        public int CreditorId { get; set; }
        public int Amount { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public int? BillId { get; set; }

        public virtual BillSplit Bill { get; set; }
        public virtual Person Creditor { get; set; }
        public virtual Person Debtor { get; set; }


        public const string DebtTransferString = "Debt Transfer: ";
        public static string CreateDebtTransferString(Person debtor, Person oldCreditor, Person newCreditor)
        {
            return string.Format("{0}{1} {2} {3}", DebtTransferString, debtor.Id, oldCreditor.Id, newCreditor.Id);
        }

        string _PrettyDescription = null;
        public string PrettyDescription
        {
            get
            {
                return _PrettyDescription ?? Description;
            }
            set
            {
                _PrettyDescription = value;
            }
        }

        public void SetPrettyDescription(Dictionary<int, Person> personMap)
        {
            _PrettyDescription = CreatePrettyDescription(Description, Amount, personMap);
        }

        public static string CreatePrettyDescription(string desc, int amount, Dictionary<int, Person> personMap)
        {
            if (!desc.StartsWith(DebtTransferString))
                return null;

            var splits = desc.Remove(0, DebtTransferString.Length).Split(' ');
            if (splits.Length != 3)
                return null;

            var debtor = personMap[int.Parse(splits[0])];
            var oldCreditor = personMap[int.Parse(splits[1])];
            var newCreditor = personMap[int.Parse(splits[2])];

            return string.Format("{0}{2}'s debt of {1:c} to {3} is now owed to {4}",
                DebtTransferString, amount / 100d, debtor.FirstName, oldCreditor.FirstName, newCreditor.FirstName);
        }
    }
}
