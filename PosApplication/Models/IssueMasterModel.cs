using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosApplication.Models
{
    public class IssueMasterModel
    {
        public string vSupplierID { get; set; }
        public string vPackID { get; set; }
        public string dIssueDate { get; set; }
        public string vReferenceNoAuto { get; set; }
        public string vReferenceNo { get; set; }
        public string vIssueNo { get; set; }
        public string vIssueFrom { get; set; }
        public string vIssueTo { get; set; }
        public string vReceivedBy { get; set; }
        public string vIssuedBy { get; set; }
        public double? mTotalAmount { get; set; }
        public string vRemarks { get; set; }
    }
}