using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosApplication.Models
{
    public class OpeningBalanceDetailModel
    {
        public string vReferenceNoAuto { get; set; }
        public string vProductId { get; set; }
        public string vProductName { get; set; }
        public string vPackId { get; set; }
        public string vPackName { get; set; }
        public double? mMaxQty { get; set; }
        public double? mMinQty { get; set; }
        public double? mCurrentStock { get; set; }
        public double? mLuQty { get; set; }
        public double? mSuQty { get; set; }
        public double? mPurchaseRate { get; set; }
        public double? mSalesRate { get; set; }
        public double? TotalAmount { get; set; }
        public double? mTotalQty { get; set; }
    }
}