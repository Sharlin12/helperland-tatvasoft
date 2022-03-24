using helperland.Models.Data;
using System.Collections.Generic;

namespace helperland.Models
{
    public class CustomerSideModel:LoginForgotPassModel
    {
        public IEnumerable<ServiceRequest> serviceRequests { get; set; }
        public IEnumerable<ServiceRequest> sr { get; set; }
        public IEnumerable<ServiceRequestAddress> serviceRequestAddresses { get; set; }
        public IEnumerable<ServiceRequestExtra> serviceRequestExtras { get; set; }
        public int Cancelrequestid { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public BookServiceModel bookServiceModel { get; set; }
        public Rating rate { get; set; }
        public string friendly { get; set; }
        public string ontime { get; set; }
        public string quality { get; set; }
        public IEnumerable<User> user { get; set; }
        public IEnumerable<Rating> myrate { get; set; }
        public IEnumerable<FavoriteAndBlocked> favoriteAndBlockeds { get; set; }
        public IEnumerable<FavoriteAndBlocked> favoriteAndBlockeds1 { get; set; }
        public int spid { get; set; }
        public int userid { get; set; }
        public int serviceproid { get; set; }

        public float endtime { get; set; }
    }
}
