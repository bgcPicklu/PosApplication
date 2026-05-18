using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosApplication.Models
{
    public class PurchaseMasterModel
    {
        //select dPurchaseDate,vReferenceNo,vChallanNo,mVatPercent,mTotalAmount,mTotalVatAmount,mTotalDiscount,mNetAmount,vUserName,dEntryTime from POS.tbPurchaseReceiveMaster
        public string vSupplierID { get; set; }
        public string vPackID { get; set; }
        public string dPurchaseDate { get; set; }
        public string vReferenceNoAuto { get; set; }
        public string vReferenceNo { get; set; }
        public string vChallanNo { get; set; }
        public double? mVatPercent { get; set; }
        public double? mTotalAmount { get; set; }
        public double? mTotalVatAmount { get; set; }
        public double? mTotalVatWithAmount { get; set; }
        public double? mTotalDiscount { get; set; }
        public double? mNetAmount { get; set; }
        public double? mPreviousDues { get; set; }
        public double? mPaidAmount { get; set; }
        public string vPaymentType { get; set; }
        public string vAccount_ChequeNo { get; set; }
        public string vBankName { get; set; }
        //public List<PurchaseDetailsModel> PurchaserDetailModel { get; set; }
    }
}