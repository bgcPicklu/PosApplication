using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PosApplication.Models
{
    public class ProductInfoModel
    {
        public string vProductAutoID { get; set; }
        public string vProductID { get; set; }
        public string vProductManualCode { get; set; }
        public string vProductName { get; set; }
        public string vProductUnit { get; set; }
        public string vCategoryID { get; set; }
        public string vCategoryName { get; set; }
        public string vPackId { get; set; }
        public string vPackSize { get; set; }
        public string vRackId { get; set; }
        public string vRackSize { get; set; }
        public double? vMaxLevel { get; set; }
        public double? vMinLevel { get; set; }
        public double? vLargeUnit { get; set; }
        public double? vSmallUnit { get; set; }
        public double? vTotalQty { get; set; }
        public string mSalesRate { get; set; }
        public string vStatus { get; set; }
        public string vProductImage { get; set; }
        [NotMapped]
        public HttpPostedFileWrapper productImageUpload { get; set; }
        public ProductInfoModel()
        {
            vProductImage = "~/Content/image/productDefault.png";
        }
    }
}