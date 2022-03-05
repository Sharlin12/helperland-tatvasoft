using System.ComponentModel.DataAnnotations;

namespace helperland.Models
{
    public class UserAddressModel1
    {
        [Required(ErrorMessage = "Please enter House number")]
        public string AddressLine1 { get; set; }
        [Required(ErrorMessage = "Please enter Street name")]
        public string AddressLine2 { get; set; }
        [Required(ErrorMessage = "Please enter City")]
        public string City { get; set; }
        [Required(ErrorMessage = "Please enter Postal Code")]
        public string PostalCode { get; set; }

        public string Mobile { get; set; }
    }
}
