using System.ComponentModel.DataAnnotations;
namespace CoreEmployeeCRUD.Models
{
    public class EmployeeModel
    {
        //public int id { get; set; }
        [Display(Name = "Firstname")]
        [Required(ErrorMessage = "* Firstname is required")]
        public string firstname { get; set; } = String.Empty;

        [Display(Name = "Surname")]
        [Required(ErrorMessage = "* Surname is required")]
        public string surname { get; set; } = String.Empty;

        [Display(Name = "Email ID")]
        [Required(ErrorMessage = "* Email is required")]
        public string email { get; set; } = String.Empty;

        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "* Password is required")]
        public string password { get; set; } = String.Empty;

        [Display(Name = "Department")]
        [Required(ErrorMessage = "* Department is required")]
        public string department { get; set; } = String.Empty;
    }
}
