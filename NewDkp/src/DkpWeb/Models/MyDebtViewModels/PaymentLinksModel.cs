using System;

namespace DkpWeb.Models.MyDebtViewModels
{
    public enum PaymentLinkType
    {
        Payment,
        Request,
    }

    public class PaymentLinksModel
    {
        public PaymentLinksModel(Person target, PaymentLinkType type, int amountCents)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (amountCents <= 0)
                throw new ArgumentOutOfRangeException("amountCents", "must be positive");

            this.Target = target;
            this.Type = type;
            this.AmountCents = amountCents;
        }

        public Person Target { get; private set; }
        public PaymentLinkType Type { get; private set; }
        public int AmountCents { get; private set; }
    }
}