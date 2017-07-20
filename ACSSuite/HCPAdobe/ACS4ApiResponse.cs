using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HCPACS4
{
    public class ACS4ApiResponse
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public string ResponseBody { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public ACS4ApiResponse(HttpStatusCode httpStatusCode, string responseBody)
        {
            this.HttpStatusCode = httpStatusCode;
            this.ResponseBody = responseBody;
            this.ProcessResponse();
        }


        private void ProcessResponse()
        {
            
            XNamespace nsAdept = "http://ns.adobe.com/adept";
            XElement xml = XElement.Parse(this.ResponseBody);

            if (xml.Name == "{http://ns.adobe.com/adept}error")
            {
                string tmp = xml.Attribute("data").Value;
                string[] split = tmp.Split(' ');
                this.ErrorCode = split[0];
                if (split.Length > 2)
                    this.ErrorMessage = WebUtility.UrlDecode(split[2]);

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
