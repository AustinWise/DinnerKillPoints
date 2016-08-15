using System.ComponentModel.DataAnnotations;

namespace DkpWeb.Models.PeopleViewModels
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