using System.ComponentModel.DataAnnotations;

namespace Thoughtwave.ViewModels.AccountViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email (Not public)")]
        public string Email { get; set; }

        [Required]
        [StringLength(25, 
            ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", 
            MinimumLength = 3)]
        [Display(Name = "Choose a username")]
        public string UserName { get; set; }
    }
}
