using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PosApplication.Models
{
    public class UnitInformationModel
    {
        public string vCompanyID { get; set; }
        public string vUnitID { get; set; }
        public string vUnitName { get; set; }
        public string vAddress { get; set; }
        public string vPhone { get; set; }
        public string vMobileNo { get; set; }
        public string vEmail { get; set; }
        public string vUnitInCharge { get; set; }
        public string vInChargeMobile { get; set; }
        public string dStartedDate { get; set; }
        public string vStatus { get; set; }
        public string vUnitType { get; set; }
    }
}