using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosApplication.Models
{
    public class ProductWiseStockRegisterReportModel
    {
        public string vCategoryID { get; set; }
        public string vSupplierID { get; set; }
        public string vPackSize { get; set; }
        public string vProductID { get; set; }
        public string dFromDate { get; set; }
        public string dToDate { get; set; }
    }
}