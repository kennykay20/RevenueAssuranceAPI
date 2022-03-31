using System;
using System.Collections.Generic;

namespace RevAssuranceApi.RevenueAssurance.DATA.ModelsDTO
{
       public class MandateResponse
        {
            public string acct_no { get; set; }
            public string acct_type { get; set; }
            public string last_name { get; set; }
            public string otherNames { get; set; }
            public string address_1 { get; set; }
            public string address_2 { get; set; }
            public string address_3 { get; set; }
            public int? branch_no { get; set; }
            public string branch_name { get; set; }
            public int? No_Signatures { get; set; }
            public string Signatory { get; set; }
            public string image_description { get; set; }
            public string image_id { get; set; }
            public string MandateType { get; set; }
            public byte[] Photo { get; set; }
            public DateTime? create_dt { get; set; }
            public string PhotoBase64
            {
                get
                {
                    return Photo != null ? "data:image/jpg;base64," + Convert.ToBase64String(Photo) : ""; // System.Web.VirtualPathUtility.ToAbsolute("~/Img/noimage2.jpg");

                }
            }

           // public List<LstSignaturePhoto> lstSignaturePhoto { get; set; }
          //  public List<LstMandate> lstMandate { get; set; }
        }

        public class LstSignaturePhoto
        {
            public string Signatory { get; set; }
            public string image_description { get; set; }
            public string image_id { get; set; }
            public string photo { get; set; }
            public string photo_date { get; set; }
            public string signature { get; set; }
            public string signature_date { get; set; }

        }

        
        public class LstMandate
        {
            public string Signatory { get; set; }
            public string image_description { get; set; }
            public string image_id { get; set; }
            public string mandate{ get; set; }
            public string mandate_date { get; set; }

        }


}