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
    }
}
