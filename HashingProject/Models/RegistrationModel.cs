using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HashingProject.Models
{
    public class RegistrationModel
    {
        [Required]
        [MinLength(3, ErrorMessage = "The field must have at least three character")]
        public string Name { get; set; }
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).+$", ErrorMessage = "The field must have at least one number and one character.")]
        public string Password { get; set; }
    }
}