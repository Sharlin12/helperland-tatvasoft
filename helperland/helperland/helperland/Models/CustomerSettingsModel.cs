using helperland.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace helperland.Models
{
    public class CustomerSettingsModel:LoginForgotPassModel
    {
        public string fname { get; set; }
        public string lname { get; set; }   
        public string phone { get; set; }
        public string day { get; set; }
        public string month { get; set; }
        public string year { get; set; }
        public string lang { get; set; }
        public IEnumerable<UserAddress> userAddresses { get; set; }
        public UserAddressModel1 myaddress { get; set; }
        public int deleteaddid { get; set; }
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
