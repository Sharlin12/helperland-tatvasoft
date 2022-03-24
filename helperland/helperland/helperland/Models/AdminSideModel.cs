using helperland.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace helperland.Models
{
    public class AdminSideModel
    {
        public IEnumerable<User> myuser { get; set; }
        public int iduser { get; set; }
        public IEnumerable<ServiceRequest> sr { get; set; }
        public IEnumerable<ServiceRequestAddress> sradd { get; set; }
        public IEnumerable<Rating> myrate { get; set; }

        public int usrid { get; set; }
        public String date1 { get; set; }
        public String time1 { get; set; }
        [Required(ErrorMessage = "Please enter Streat name")]
        public String Add1 { get; set; }
        [Required(ErrorMessage = "Please enter House number")]
        public String Add2 { get; set; }
        [Required(ErrorMessage = "Please enter PostalCode")]
        public String zipcode { get; set; }
        [Required(ErrorMessage = "Please enter City")]
        public String City { get; set; }
        public int Srid { get; set; }
    }
}
