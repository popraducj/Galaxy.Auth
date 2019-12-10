using System.ComponentModel.DataAnnotations;

namespace Galaxy.Auth.Presentation.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [MaxLength(256)]
        public string Id { get; set; }
        
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}