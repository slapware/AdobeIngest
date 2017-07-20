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
    public class PackageRequest
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Galley's are tagged with this specific metadata
        // Ultimately to prevent a non Galley from getting overwritten by a Galley
        const string GALLEY_PUBLISHER = "Harper Collins eGalley - NOT for re-sale"; //TODO: Move this pull from a  config file
        const string REQUEST_ENDPOINT = "/packaging/Package";


        public Epub Epub { get; set; }

        public PackageRequest(Epub epub)
        {
            this.Epub = epub;
        }
        

        public PackageRequestResponse SendRequest()
        {
            PackageRequestResponse packageResponse = new PackageRequestResponse();
            packageResponse.FileName = this.Epub.FullName;

            Stream dataStream = null;
            try
            {
                XElement packageData = this.BuildRequest(this.Epub.IsGalley, this.Epub.ACS4Guid);
                string epubBase64Encoded = this.Epub.GetEpubAsBase64();

                // inject book data into package request
                XNamespace nsAdept = "http://ns.adobe.com/adept";
                packageData.Add(new XElement(nsAdept + "data", epubBase64Encoded));

                Byte[] secretBytes = Convert.FromBase64String(HCPACS4.AdobeConfig.ACS_ECTB_BUILTIN_SECRET_KEY);
                string signedData = Adobe.Adept.XmlUtils.GetSignedMessage(packageData.ToString(), secretBytes);
                byte[] byteArray = Encoding.UTF8.GetBytes(signedData);
                signedData = null;


                // Set up the web request and send the web request
                WebRequest request = WebRequest.Create(HCPACS4.AdobeConfig.ACS_PACKAGING_SERVER + REQUEST_ENDPOINT);
                request.Method = "POST";
                request.ContentType = "application/vnd.adobe.adept+xml";

                log.Info(String.Format("Sending PackagingRequest for {0} : {1}", this.Epub.DropFolderAlias, this.Epub.FullName));
                request.ContentLength = byteArray.Length;
                dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);

                using (WebResponse response = request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    string responseFromServer = reader.ReadToEnd();
                    packageResponse.ProcessResponse(((HttpWebResponse)response).StatusCode, responseFromServer);
                }
            }
            catch (System.IO.IOException exNetworkIO)
            {
                log.Error(String.Format("System.IO.IOException sending PackagingRequest for {0} : {1}", this.Epub.DropFolderAlias, this.Epub.FullName), exNetworkIO);
                packageResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
                packageResponse.ErrorMessage = exNetworkIO.Message;
            }
            catch (System.Net.Sockets.SocketException exSockets)
            {
                log.Error(String.Format("System.Net.Sockets.SocketException sending PackagingRequest for {0} : {1}", this.Epub.DropFolderAlias, this.Epub.FullName), exSockets);
                packageResponse.ErrorMessage = exSockets.Message;
                packageResponse.HttpStatusCode = HttpStatusCode.InternalServerError;
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Unhandled exception sending PackagingRequest for {0} : {1}", this.Epub.DropFolderAlias, this.Epub.FullName), ex);
                packageResponse.ErrorMessage = ex.Message;
            }
            finally
            {
                if (dataStream != null)
                    dataStream.Close();
            }

            return packageResponse;
        }

     

        private XElement BuildRequest(bool isGalley, string ACS4Guid)
        {
            XNamespace nsAdept = "http://ns.adobe.com/adept";
            XNamespace nsdc = "http://purl.org/dc/elements/1.1/";
            
            XElement package = null; 
            XElement metadata = null;
            XElement permissions = new XElement(nsAdept + "permissions",
                new XElement(nsAdept + "display"),
                new XElement(nsAdept + "print",
                        new XElement(nsAdept + "count", new XAttribute("incrementInterval", "604800"), new XAttribute("initial", "10")),
                        new XElement(nsAdept + "maxResolution", "300")
                    )
            );

            //TODO: This next line confusing me.. may not be needed... review later
            //package = new XElement(nsAdept + "package", new XAttribute("action", "add"));
            if (String.IsNullOrEmpty(ACS4Guid))
                package = new XElement(nsAdept + "package", new XAttribute("action", "add"));
            else
            {
                package = new XElement(nsAdept + "package", new XAttribute("action", "replace"),
                            new XElement(nsAdept + "resource", ACS4Guid)
                        );
            }

            //TODO: I think I need to handle the scenario when there is no dc:identifier in .opf of the epub
            //          as I think this would mean the book would not be found in the catalog when trying to find isbn

            //// Galleys get tagged by a specific value in the <dc:publisher> node
            if (isGalley)
            {
                metadata = new XElement(nsAdept + "metadata", new XAttribute(XNamespace.Xmlns + "dc", nsdc),
                                new XElement(nsdc + "publisher", GALLEY_PUBLISHER)
                            );  
            }
            if (metadata != null)
            {
                package.Add(metadata);
            }

            package.Add(permissions);

            return package;

        }
    }

   
}
