using System;
using System.Collections.Generic;

namespace DkpWeb.Models
{
    public partial class PaymentMethod
    {
        public PaymentMethod()
        {
            PaymentIdentity = new HashSet<PaymentIdentity>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string PayLinkFormat { get; set; }
        public string RequestMoneyLinkFormat { get; set; }

        public virtual ICollection<PaymentIdentity> PaymentIdentity { get; set; }
    }
}
