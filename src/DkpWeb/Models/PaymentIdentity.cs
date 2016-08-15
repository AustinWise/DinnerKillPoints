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


        public string CreatePayLink(int amountCents)
        {
            return createLink(this.PaymentMeth.PayLinkFormat.Trim(), amountCents);
        }

        public string CreateRequestMoneyLink(int amountCents)
        {
            return createLink(this.PaymentMeth.RequestMoneyLinkFormat.Trim(), amountCents);
        }

        private string createLink(string format, int amountCents)
        {
            if (amountCents <= 0)
                throw new ArgumentOutOfRangeException("amountCents", "must be positive");
            if (string.IsNullOrEmpty(format))
                throw new NotSupportedException("");

            var param = new Formattable[]
            {
                new Formattable(UserName),
                new Formattable((amountCents / 100d).ToString("0.00")),
            };

            return string.Format(format, param);
        }
    }
}
