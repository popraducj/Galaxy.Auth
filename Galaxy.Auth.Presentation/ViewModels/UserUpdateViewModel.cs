using System.ComponentModel.DataAnnotations;

namespace Galaxy.Auth.Presentation.ViewModels
{
    public class UserUpdateViewModel
    {
        [Required]
        public string Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        public string Phone { get; set; }
    }
}