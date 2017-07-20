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
    public class ManageDistributionRights
    {
        public string DistributorId { get; set; }
        public string ACS4Guid { get; set; }
        public string SecretKey { get; set; }

        const string REQUEST_ENDPOINT = "/admin/ManageDistributionRights";

        public ManageDistributionRights(string distributorId, string ACS4Guid)
        {
            this.DistributorId = distributorId;
            this.ACS4Guid = ACS4Guid;
        }

        public ACS4ApiResponse AssignRights()
        {

            XElement requestData = this.BuildAssignRightsRequest(this.DistributorId, this.ACS4Guid);
            Byte[] secretBytes = Convert.FromBase64String(HCPACS4.AdobeConfig.ACS_ECTB_BUILTIN_SECRET_KEY);
            string signedData = Adobe.Adept.XmlUtils.GetSignedMessage(requestData.ToString(), secretBytes);
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

        public ACS4ApiResponse RemoveRights()
        {
            XElement requestData = this.BuildDeleteRightsRequest(this.DistributorId, this.ACS4Guid);
            Byte[] secretBytes = Convert.FromBase64String(HCPACS4.AdobeConfig.ACS_ECTB_BUILTIN_SECRET_KEY);
            string signedData = Adobe.Adept.XmlUtils.GetSignedMessage(requestData.ToString(), secretBytes);
            byte[] byteArray = Encoding.UTF8.GetBytes(signedData);
            
            
            WebRequest request = WebRequest.Create(HCPACS4.AdobeConfig.ACS_PACKAGING_SERVER + REQUEST_ENDPOINT);
            request.Method = "POST";
            request.ContentType = "application/vnd.adobe.adept+xml";

            //TODO: Try Catch
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            ACS4ApiResponse apiResponse;
            string responseFromServer;
            using (WebResponse response = request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(responseStream))
            {
                responseFromServer = reader.ReadToEnd();
                apiResponse = new ACS4ApiResponse(((HttpWebResponse)response).StatusCode, responseFromServer);
            }

            return apiResponse;
        }

        private XElement BuildAssignRightsRequest(string distributorId, string ACS4Guid)
        {
            XNamespace nsAdept = "http://ns.adobe.com/adept";
            XNamespace nsdc = "http://purl.org/dc/elements/1.1/";

            XElement request = new XElement(nsAdept + "request", new XAttribute("action", "create"),
                new XElement(nsAdept + "distributionRights",
                    new XElement(nsAdept + "distributor", distributorId),
                    new XElement(nsAdept + "resource", ACS4Guid),
                    new XElement(nsAdept + "distributionType", "buy"),
                    new XElement(nsAdept + "available", "2"),
                    new XElement(nsAdept + "returnable", "false"),
                    new XElement(nsAdept + "userType", "user"),
                    new XElement(nsAdept + "permissions",
                        new XElement(nsAdept + "display"),
                        new XElement(nsAdept + "play"),
                        new XElement(nsAdept + "excerpt"),
                        new XElement(nsAdept + "print"))

            ));

            return request;


        }

        private XElement BuildDeleteRightsRequest(string distributorId, string ACS4Guid)
        {
            XNamespace nsAdept = "http://ns.adobe.com/adept";
            XNamespace nsdc = "http://purl.org/dc/elements/1.1/";
 
            XElement request = new XElement(nsAdept + "request", new XAttribute("action", "delete"), new XAttribute("auth", "builtin"),
                new XElement(nsAdept + "distributionRights",
                    new XElement(nsAdept + "distributor", distributorId),
                    new XElement(nsAdept + "resource", ACS4Guid)
            ));

            return request;


        }
    }
}
