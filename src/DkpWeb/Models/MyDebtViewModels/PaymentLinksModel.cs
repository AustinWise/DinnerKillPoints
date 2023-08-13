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
        public PaymentLinksModel(Person target, PaymentLinkType type, Money amount)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (amount <= Money.Zero)
                throw new ArgumentOutOfRangeException(nameof(amount), "must be positive");

            this.Target = target;
            this.Type = type;
            this.Amount = amount;
        }

        public Person Target { get; private set; }
        public PaymentLinkType Type { get; private set; }
        public Money Amount { get; private set; }
    }
}