using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HCPACS4
{
    class ManageResourceItem
    {
        const string REQUEST_ENDPOINT = "/admin/ManageResourceItem";


        public string ACS4Guid { get; set; }

        public ManageResourceItem(string ACS4Guid)
        {
            this.ACS4Guid = ACS4Guid;
        }
        

        /// <summary>
        /// Only used for DEV purposes to clear out epubs already ingested
        /// Note: This does not currently delete from distributors, which would be required for anything already assigned toa distributor
        ///  
        /// </summary>
        /// <returns></returns>
        public ACS4ApiResponse Delete()
        {

            // build metadata for package request
            XElement requestData = this.BuildDeleteRequest();

            Byte[] secretBytes = Convert.FromBase64String(HCPACS4.AdobeConfig.ACS_ECTB_BUILTIN_SECRET_KEY);

            string signedData = Adobe.Adept.XmlUtils.GetSignedMessage(requestData.ToString(), secretBytes);

            // Encode the request as a byte array
            byte[] byteArray = Encoding.UTF8.GetBytes(signedData);

            // Set up the web request and send the web request
            WebRequest request = WebRequest.Create(HCPACS4.AdobeConfig.ACS_PACKAGING_SERVER + REQUEST_ENDPOINT);
            request.Method = "POST";
            request.ContentType = "application/vnd.adobe.adept+xml";

            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();


            ACS4ApiResponse apiResponse;
            string responseFromServer;
            // Write the response out to the console
            using (WebResponse response = request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(responseStream))
            {
                responseFromServer = reader.ReadToEnd();
                apiResponse = new ACS4ApiResponse(((HttpWebResponse)response).StatusCode, responseFromServer);
            }

            return apiResponse;
        }


        private XElement BuildDeleteRequest()
        {

            XNamespace nsAdept = "http://ns.adobe.com/adept";
            XNamespace nsdc = "http://purl.org/dc/elements/1.1/";

            XElement request = new XElement(nsAdept + "request", new XAttribute("action", "delete"), new XAttribute("auth", "builtin"),
                new XElement(nsAdept + "resourceItemInfo",
                    new XElement(nsAdept + "resource", this.ACS4Guid),
                    new XElement(nsAdept + "resourceItem", 0)
                    
            ));

            return request;

        }
    }
}
