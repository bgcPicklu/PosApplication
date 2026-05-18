using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosApplication.Models
{
    public class SalesDetailsModel
    {
        public string vUnitID { get; set; }
        public string vInvoiceNo { get; set; }
        public string vProductID { get; set; }
        public string vProductName { get; set; }
        public string vPackSize { get; set; }
        public decimal? mMaxQty { get; set; }
        public decimal? mMinQty { get; set; }
        public decimal? mCurrentStock { get; set; }
        public decimal? mLuQty { get; set; }
        public decimal? mSuQty { get; set; }
        public decimal? mSalesQty { get; set; }
        public decimal? mReturnLuQty { get; set; }
        public decimal? mReturnSuQty { get; set; }
        public decimal? mReturnSalesQty { get; set; }
        public decimal? mPurchaseRate { get; set; }
        public decimal? mSalesRate { get; set; }
        public decimal? mAmount { get; set; }
    }
}