using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosApplication.Models
{
    public class SalesMasterModel
    {
        public string vUnitID { get; set; }
        public string vInvoiceNoAuto { get; set; }
        public string vInvoiceNo { get; set; }
        public string vChallanNo { get; set; }
        public string dInvoiceDate { get; set; }
        public string vSupplierID { get; set; }
        public string vPackID { get; set; }
        public string vReceiverName { get; set; }
        public string vSoldBy { get; set; }
        public string mNetAmount { get; set; }
        public string vPaymentType { get; set; }
        public string vAccount_ChequeNo { get; set; }
        public string vBankName { get; set; }
    }
}