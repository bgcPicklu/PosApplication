using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosApplication.Models
{
    public class ReportModel
    {
        public string vSupplierID { get; set; }
        public string vUnitID { get; set; }
        public string vProductID { get; set; }
        public string dFromDate { get; set; }
        public string dToDate { get; set; }
    }
}