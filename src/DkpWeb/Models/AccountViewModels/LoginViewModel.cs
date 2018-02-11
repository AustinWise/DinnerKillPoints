using System.ComponentModel.DataAnnotations;

namespace DkpWeb.Models.AccountViewModels
{
    public class LoginViewModel
    {
        public string Email { get; set; }

        public bool RememberMe { get; set; }
    }
}
