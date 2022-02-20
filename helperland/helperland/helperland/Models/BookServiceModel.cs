using helperland.Models.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace helperland.Models
{
    public class BookServiceModel: LoginForgotPassModel
    {
        public ZipCodeMatchingModel ZipCodeMatching { get; set; }
        public ScheduleAndPlanModel sap{get;set; }
        public IEnumerable<UserAddress> ua { get; set; }
        public UserAddressModel UserAddressModel { get; set; }
        [Required]
        public bool privacycheck { get; set; }
        public string selectedaddress { get; set; }
    }
}
