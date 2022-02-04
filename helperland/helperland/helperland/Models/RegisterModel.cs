using helperland.Models.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace helperland.Models
{
    [NotMapped]
    public partial class RegisterModel : User
    {
        public int UserId { get; set; }
        [Required(ErrorMessage = "Please enter fname")]
        public String FirstName { get; set; }
        [Required(ErrorMessage = "Please enter lname")]
        public String LastName { get; set; }
        [Required(ErrorMessage = "Please enter email")]
        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)\S{8,10}$", ErrorMessage = "8-10 characters,1 Alphabet, 1 Number, 1 Special Character")]
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public String Password { get; set; }
        [Required(ErrorMessage = "Please enter mobile")]
        public String Mobile { get; set; }
        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Please enter confirm password")]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        [DataType(DataType.Password)]
        public String Confirmpwd { get; set; }

        [Required]
        public bool privacyPolicy { get; set; }


    }
}