using System.ComponentModel.DataAnnotations;

namespace Transport.REST.Models
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Поле 'Email' є обов'язковим.")]
        [EmailAddress(ErrorMessage = "Будь ласка, введіть коректний Email.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле 'Пароль' є обов'язковим.")]
        public string Password { get; set; } = string.Empty;
    }
}