using System.ComponentModel.DataAnnotations;

namespace Transport.REST.Models
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Поле 'Ім'я' є обов'язковим.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле 'Прізвище' є обов'язковим.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле 'Email' є обов'язковим.")]
        [EmailAddress(ErrorMessage = "Будь ласка, введіть коректний Email.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле 'Пароль' є обов'язковим.")]
        [MinLength(8, ErrorMessage = "Пароль повинен містити щонайменше 8 символів.")]
        public string Password { get; set; } = string.Empty;
    }
}