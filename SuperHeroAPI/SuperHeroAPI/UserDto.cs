using System.ComponentModel.DataAnnotations;

namespace SuperHeroAPI
{
    public class UserDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int CountryId { get; set; }
    }
}
