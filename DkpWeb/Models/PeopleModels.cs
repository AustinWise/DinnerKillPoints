using Austin.DkpLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DkpWeb.Models
{
    public class AddPaymentMethodModel
    {
        [Required]
        [Display(Name = "Payment Method")]
        public int PaymentMethodId { get; set; }

        [Required(AllowEmptyStrings = false)]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
    }
}