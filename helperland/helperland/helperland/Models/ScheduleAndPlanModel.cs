using System;
using System.ComponentModel.DataAnnotations;
namespace helperland.Models
{
    public class ScheduleAndPlanModel
    {
        
        public string YourDate { get; set; }
        public string yourtime { get; set; }
        public string serviceHrs { get;set;}
        public string extraHrs { get; set; }
        public string subtotal { get; set; }
        public string totalcost { get;set; }
        public string comments{ get;set; }
        public bool haspets { get; set; }
        public bool extra1 { get; set; }
        public bool extra2{ get; set; }
        public bool extra3 { get; set; }
        public bool extra4 { get; set; }
        public bool extra5 { get; set; }

    }
}
