using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosApplication.Models
{
    public class PurchaseDetailsModel
    {
        public string vSupplierID { get; set; }
        public string vPackSize { get; set; }
        public string dPurchaseDate { get; set; }
        public string vReferenceNo { get; set; }
        public string vProductID { get; set; }
        public string vProductName { get; set; }
        public string vPackId { get; set; }
        public string vPackName { get; set; }
        public double? mMaxQty { get; set; }
        public double? mMinQty { get; set; }
        public double? mCurrentStock { get; set; }
        public double? mLuQty { get; set; }
        public double? mSuQty { get; set; }
        public double? mTotalQty { get; set; }
        public double? mAmount { get; set; }
        public double? mVat { get; set; }
        public double? mDiscount { get; set; }
        public double? mNetAmount { get; set; }
        public double? mPurchaseRate { get; set; }
        public double? mSalesRate { get; set; }
    }
}