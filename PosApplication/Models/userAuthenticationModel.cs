using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PosApplication.Models
{
    public class userAuthenticationModel
    {
        public string vUnitID { get; set; }
        public string vUserName { get; set; }
        public string vMenuID { get; set; }
        public string vMenuName { get; set; }
        public string vMenuType { get; set; }
        public int iShow { get; set; }
        public int iSave { get; set; }
        public int iEdit { get; set; }
        public int iDelete { get; set; }
    }
}