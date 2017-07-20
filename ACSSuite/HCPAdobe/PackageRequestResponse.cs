using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HCPACS4
{

    public class PackageRequestResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string ResponseBody { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public bool Ingested { get; set; } 
        public string FileName { get; set; }
        public string ACS4Guid { get; set; }


        public void ProcessResponse(HttpStatusCode httpStatusCode, string responseBody)
        {
            this.ResponseBody = responseBody;
            this.HttpStatusCode = httpStatusCode;

            XNamespace nsAdept = "http://ns.adobe.com/adept";
            XElement xml = XElement.Parse(responseBody);

            this.Ingested = false;
            if (xml.Name == "{http://ns.adobe.com/adept}error")
            {
                string tmp = xml.Attribute("data").Value;
                string[] split = tmp.Split(' ');
                this.ErrorCode = split[0];
                if (split.Length > 2)
                    this.ErrorMessage = WebUtility.UrlDecode(split[2]);

               
            }
            else
            {
                this.Ingested = true;
                this.ACS4Guid = xml.Element(nsAdept + "resource").Value;
            }

        }

        public bool IsError()
        {
            if (!String.IsNullOrEmpty(this.ErrorCode) || !String.IsNullOrEmpty(this.ErrorMessage))
                return true;
            else
                return false;
        }
    }

   
}
