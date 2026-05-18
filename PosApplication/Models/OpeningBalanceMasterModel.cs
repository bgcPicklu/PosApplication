using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosApplication.Models
{
    public class OpeningBalanceMasterModel
    {
        public string vUnitID { get; set; }
        public string vReferenceNo { get; set; }
        public string vReferenceNoAuto { get; set; }
        public string vChallanNo { get; set; }
        public string dOpeningDate { get; set; }
        public string vSupplierId { get; set; }
        public double? mTotalAmount { get; set; }
        public string vPackSize { get; set; }
        public string vRemarks { get; set; }
    }
}