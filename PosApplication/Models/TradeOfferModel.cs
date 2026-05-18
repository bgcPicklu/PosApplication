using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosApplication.Models
{
    public class TradeOfferModel
    {
        public string vProductID { get; set; }
        public string vProductName { get; set; }
        public decimal nShortUnitQty { get; set; }
        public decimal nAmount { get; set; }
    }
}