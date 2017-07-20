using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace HCPACS4
{
    public class ResourceItemInfoCollection
    {
        public ACS4ApiResponse FetchCatalog(string distributorId, string secretKey)
        {
            
            //TODO: Refactor building of XML with XElement
            string endPoint = "/admin/QueryResourceItems";
            string postdata = "<request xmlns=\"http://ns.adobe.com/adept\">" + 
                                  "<distributor>{0}</distributor>" +
                                  "<QueryResourceItems/>" + 
                              "</request>";

            
            postdata = String.Format(postdata, distributorId);
            Byte[] secretBytes = Convert.FromBase64String(secretKey);

            string signedData = Adobe.Adept.XmlUtils.GetSignedMessage(postdata, secretBytes);
            

            // Encode the request as a byte array
            byte[] byteArray = Encoding.UTF8.GetBytes(signedData);

            // Set up the web request and send the web request
            WebRequest request = WebRequest.Create(HCPACS4.AdobeConfig.ACS_PACKAGING_SERVER + endPoint);
            request.Method = "POST";
            request.ContentType = "application/vnd.adobe.adept+xml";

            request.ContentLength = byteArray.Length;
            //TODO: Need exception handling here -- just disconnect from network here to try it out.
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
    }
}
