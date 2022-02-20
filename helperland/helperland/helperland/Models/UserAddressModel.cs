using System.ComponentModel.DataAnnotations;

namespace helperland.Models
{
    public class UserAddressModel
    {
        [Required(ErrorMessage = "Please enter Street name")]
        public string AddressLine1 { get; set; }
        [Required(ErrorMessage = "Please enter House number")]
        public string AddressLine2 { get; set; }
        public string City { get; set; }
      
        public string PostalCode { get; set; }
      
        public string Mobile { get; set; }
    }
}
