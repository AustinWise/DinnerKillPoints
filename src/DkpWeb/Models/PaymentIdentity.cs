using Austin.DkpLib;
using System;
using System.Collections.Generic;

namespace DkpWeb.Models
{
    public partial class PaymentIdentity
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int PaymentMethId { get; set; }
        public string UserName { get; set; }

        public virtual PaymentMethod PaymentMeth { get; set; }
        public virtual Person Person { get; set; }


        public string CreatePayLink(Money amount)
        {
            return createLink(this.PaymentMeth.PayLinkFormat.Trim(), amount);
        }

        public string CreateRequestMoneyLink(Money amount)
        {
            return createLink(this.PaymentMeth.RequestMoneyLinkFormat.Trim(), amount);
        }

        private string createLink(string format, Money amount)
        {
            if (amount <= Money.Zero)
                throw new ArgumentOutOfRangeException(nameof(amount), "must be positive");
            if (string.IsNullOrEmpty(format))
                throw new NotSupportedException("");

            var param = new Formattable[]
            {
                new Formattable(UserName),
                new Formattable(amount.ToString(Money.FORMAT_BARE, null)),
            };

            string ret = string.Format(format, param);

            return ret;
        }
    }
}
