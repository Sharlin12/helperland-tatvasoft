using helperland.Models.Data;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace helperland.Models
{
    public class ContactUsModel:ContactU
    {
        [Required(ErrorMessage = "Please enter first name")]

        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter last name")]

        public string LastName { get; set; }
        [Required(ErrorMessage = "Please enter Email")]

        public string Email { get; set; }


        public string Subject { get; set; }
        [Required(ErrorMessage = "Please enter Phone-no")]

        public string Mobile { get; set; }
        [Required(ErrorMessage = "Please enter message")]

        public string Message { get; set; }

        public string UploadFileName { get; set; }
        public string FileName { get; set; }
        public IFormFile upload { get; set; }
    }
}

