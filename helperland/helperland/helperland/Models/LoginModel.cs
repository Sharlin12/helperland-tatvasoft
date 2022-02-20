using helperland.Models.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace helperland.Models
{
    [NotMapped]
    public partial class LoginModel : User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Please enter email")]
        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }


        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public String Password { get; set; }
        
        public String hiddenfield { get; set; }

    }
}
