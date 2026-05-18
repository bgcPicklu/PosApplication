using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PosApplication.Models
{
    public class CompanyInformationModel
    {
        public string vCompanyName { get; set; }
        public string vAddress { get; set; }
        public string vPhone { get; set; }
        public string vMobile { get; set; }
        public string vEmail { get; set; }
        public string vLogo { get; set; }
        [NotMapped]
        public HttpPostedFileWrapper LogoUpload { get; set; }
        public CompanyInformationModel()
        {
            vLogo = "~/Content/image/default.png";
        }
    }
}