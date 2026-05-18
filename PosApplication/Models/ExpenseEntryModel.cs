using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosApplication.Models
{
    public class ExpenseEntryModel
    {
        public string dDate { get; set; }
        public string vUnitID { get; set; }
        public string vTransactionIDAuto { get; set; }
        public string vTransactionID { get; set; }
        public string vChallanNo { get; set; }
        public string vAccountHeadID { get; set; }
        public string vAccountHead { get; set; }
        public decimal nAmount { get; set; }
    }
}