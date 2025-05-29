using System.ComponentModel.DataAnnotations;

namespace Transport.REST.Models
{
    public class AssignRoleRequest
    {
        [Required(ErrorMessage = "Поле 'Email' є обов'язковим.")]
        [EmailAddress(ErrorMessage = "Будь ласка, введіть коректний Email.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Поле 'Назва ролі' є обов'язковим.")]
        public string RoleName { get; set; } = string.Empty;
    }
}