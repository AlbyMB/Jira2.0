using System.ComponentModel.DataAnnotations;

namespace Final_Grp6_PROG3340_UI.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
