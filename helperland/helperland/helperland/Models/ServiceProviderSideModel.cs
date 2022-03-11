using helperland.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace helperland.Models
{
    public class ServiceProviderSideModel:LoginForgotPassModel
    {
        public IEnumerable<ServiceRequest> serviceRequests { get; set; }
        public IEnumerable<ServiceRequest> serviceRequestswithoutpets { get; set; }
        public IEnumerable<ServiceRequestAddress> serviceRequestAddresses { get; set; }
        public IEnumerable<ServiceRequestExtra> serviceRequestExtras { get; set; }
        public IEnumerable<FavoriteAndBlocked> favoriteAndBlockeds { get; set; }
        public IEnumerable<FavoriteAndBlocked> favoriteAndBlockeds1 { get; set; }
        public IEnumerable<User> users { get; set; }
        public IEnumerable<Rating> ratings { get; set; }
        public int srid { get; set; }
        public DateTime starttime { get; set; }
        public DateTime endtime { get; set; }
        public int spid { get; set; }
        public int userid { get; set; }

        public string fname { get; set; }
        public string lname { get; set; }
        public string phone { get; set; }
        public string day { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string natid { get; set; }
        public int gender { get; set; }
        public string add1 { get; set; }
        public string add2 { get; set; }
        public string city { get; set; }
        public string postal { get; set; }
        public string profiledp { get; set; }


        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)\S{8,10}$", ErrorMessage = "8-10 characters,1 Alphabet, 1 Number, 1 Special Character")]
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Please enter confirm password")]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        [DataType(DataType.Password)]
        public string Confirmpwd { get; set; }
        [Required(ErrorMessage = "Please enter password")]
        [DataType(DataType.Password)]
        public string oldpassword { get; set; }
    }
}
