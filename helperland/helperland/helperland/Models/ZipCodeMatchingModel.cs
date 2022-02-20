using System.ComponentModel.DataAnnotations;
namespace helperland.Models
{
    public partial class ZipCodeMatchingModel
    {
        [Required]
        public string PostalCode { get; set; }
    }
}
