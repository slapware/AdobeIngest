using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using MySql.Data.MySqlClient;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.XPath;


namespace ACS4Ingest
{
  class DistributorManager
  {
    const string builtin_distributor_idstring = "00000000000000000000000000000001";
    
    /////////////////////////////////////////////////////////////////////////////
    // Change this information to correspond to your server setup  
    //const string serverURL = "http://dans-win7.dlogics.com:8080"; // Tomcat URL, no trailing slash
    const string serverURL = "http://hcussc159"; // Tomcat URL, no trailing slash
    const string serverName = "127.0.0.1";  // database domain or IP (no port number)
    const string dbName = "";  // database name
    const string userName = "";  // database user name
    const string sqlPass = "";  // database password
    public Byte[] secretBytes = Convert.FromBase64String("D5bpNyV2aEEX3FIvTPGFU28upV0=");  // shared secret for built-in distributor (optional)
    /////////////////////////////////////////////////////////////////////////////
    
    public Distributor my_distributor;

    public DistributorManager()
    {
      my_distributor = new Distributor();
    }

    public void GetAllDistributors()
    {

        // For this request, we need the builtin distributor's information
        // ************** Uncomment these two lines if you DID NOT fill in the shared secret above **************
        //GetCertainDistributorFromDB("builtin");
        //byte[] mysecret = my_distributor.SharedSecret;

        // ************** Uncomment this line if you DID fill in the shared secret above **************
        byte[] mysecret = secretBytes;

        // This request will get all the distributor data responses
        // Note that request expiration, nonce, and HMAC are not added at this point
        string postdata = "<request action=\"get\" auth=\"builtin\" xmlns=\"http://ns.adobe.com/adept\">" +
                          "<distributorData>" +
                          "</distributorData>" +
                          "</request>";

        // Pass the data and the shared secret to the Adobe utility
        // Shared secret is a byte array; here, we got the secret by querying the database (see above)
        //string signedData = Adobe.Adept.XmlUtils.GetSignedMessage(postdata, my_distributor.SharedSecret);
        string signedData = Adobe.Adept.XmlUtils.GetSignedMessage(postdata, mysecret);
        Console.WriteLine(signedData);

        // Encode the request as a byte array
        byte[] byteArray = Encoding.UTF8.GetBytes(signedData);

        // Set up the web request and send the web request
        WebRequest request = WebRequest.Create(serverURL + "/admin/ManageDistributor");
        request.Method = "POST";
        request.ContentType = "application/vnd.adobe.adept+xml";

        request.ContentLength = byteArray.Length;
        Stream dataStream = request.GetRequestStream();
        dataStream.Write(byteArray, 0, byteArray.Length);
        dataStream.Close();

        string responseFromServer;

        // Write the response out to the console
        using (WebResponse response = request.GetResponse())
        using (Stream responseStream = response.GetResponseStream())
        using (StreamReader reader = new StreamReader(responseStream))
        {
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.

            responseFromServer = reader.ReadToEnd();
            // Display the content.
            Console.WriteLine(responseFromServer);
        }

        return;
    }

    public void GetCertainDistributorFromDB(string certainName)
    {

        // Set up the database connection
        string MyConString = "SERVER="+ serverName +";" + "DATABASE="+ dbName +";" + "UID="+ userName +";" + "PASSWORD="+ sqlPass +";";
        MySqlConnection connection = new MySqlConnection(MyConString);
        MySqlCommand command = connection.CreateCommand();
        MySqlDataReader Reader;

        // The name is CASE INSENSITIVE, but must otherwise be exact
        Console.WriteLine("The name to look for is: " + certainName);

        // Get everything from the distributor table
        command.CommandText = "SELECT * FROM distributor";
        connection.Open();
        Reader = command.ExecuteReader();
        while (Reader.Read())
        {
            byte[] distid = (byte[])Reader.GetValue(0);
            string distname = Reader.GetValue(1).ToString();

            // Find the name we're looking for and get all the details we need
            if (distname.Trim().ToUpper().Equals(certainName.Trim().ToUpper()))
            {
                byte[] bytes = (byte[])Reader.GetValue((int)Distributor.DistributeDBColumnIndex.DIST_ID);
                my_distributor.DistributorID = Adobe.Adept.Utils.ByteArrayToString(bytes);
                my_distributor.DistributorName = Reader.GetValue((int)Distributor.DistributeDBColumnIndex.DIST_NAME).ToString();
                my_distributor.SharedSecret = (byte[])Reader.GetValue((int)Distributor.DistributeDBColumnIndex.DIST_SHAREDSECRET); ;
                my_distributor.LinkExpiration = (int)Reader.GetValue((int)Distributor.DistributeDBColumnIndex.DIST_LINKEXPIRATION);
            }
        }

        Reader.Close();
        connection.Close();
    }
  }
}
