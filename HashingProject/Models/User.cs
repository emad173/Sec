using System.ComponentModel.DataAnnotations;

namespace HashingProject.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d).+$", ErrorMessage = "The field must have at least one number and one character.")]
        public string Password { get; set; }
        public string Salt { get; set; }
    }
}