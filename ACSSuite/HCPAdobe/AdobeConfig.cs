using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace HCPACS4
{
    class AdobeConfig
    {

        //ConnectionStringSettings _connString = ConfigurationManager.ConnectionStrings["DDRContext"];
        //public const string apikey = ConfigurationManager.AppSettings["serverUrl"];

        /* PROD */
        //public const string ACS_PACKAGING_SERVER = "http://hcussc159"; // Tomcat URL, no trailing slash
        //public const string ACS_ECTB_DISTRIBUTORID = "urn:uuid:cfb61c18-c32b-488e-a523-99eeaa0c739a";    
        //public const string ACS_ECTB_SECRET_KEY = "D5bpNyV2aEEX3FIvTPGFU28upV0="; 
        //public const string ACS_ECTB_BUILTIN_DISTRIBUTORID = "urn:uuid:00000000-0000-0000-0000-000000000001";    //builtin
        //public const string ACS_ECTB_BUILTIN_SECRET_KEY = "oQGVwqtFSvaEFsGcidSBoriTn8k="; //builtin
        //public const string AZURE_STORAGE_ACCT = "DefaultEndpointsProtocol=https;AccountName=hcpetgectb;AccountKey=5EqPUhsTtGtiIs8ZZUSzvSWYod/IHOAxPEeWXo4UnSst8WulZIfjXMq5hc9+S9VUPDproCK58TklTnJ2FlxeaA==;";
        //public const string AZURE_BLOB_CONTAINER = "ectbingestarchive";

        //public const string ACS_PACKAGING_SERVER = ConfigurationManager.AppSettings["ACS_PACKAGING_SERVER"];
        //public const string ACS_ECTB_DISTRIBUTORID = ConfigurationManager.AppSettings["ACS_ECTB_DISTRIBUTORID"];
        //public const string ACS_ECTB_SECRET_KEY = ConfigurationManager.AppSettings["ACS_ECTB_SECRET_KEY"];
        //public const string ACS_ECTB_BUILTIN_DISTRIBUTORID = ConfigurationManager.AppSettings["ACS_ECTB_BUILTIN_DISTRIBUTORID"];
        //public const string ACS_ECTB_BUILTIN_SECRET_KEY = ConfigurationManager.AppSettings["ACS_ECTB_BUILTIN_SECRET_KEY"];
        //public const string AZURE_STORAGE_ACCT = ConfigurationManager.AppSettings["AZURE_STORAGE_ACCT"];
        //public const string AZURE_BLOB_CONTAINER = ConfigurationManager.AppSettings["AZURE_BLOB_CONTAINER"];


        //// local directory to stage epubs during ingestion
        //public const string LOCAL_STAGING_PATH = ConfigurationManager.AppSettings["LOCAL_STAGING_PATH"];
        //public const int BATCH_SIZE = Convert.ToInt32(ConfigurationManager.AppSettings["BATCH_SIZE"]);
        //public const bool DELETE_FROM_FTP = Convert.ToBoolean(ConfigurationManager.AppSettings["DELETE_FROM_FTP"]);
        //public const int MAX_FILE_SIZE = Convert.ToInt32(ConfigurationManager.AppSettings["MAX_FILE_SIZE"]);


        //public const bool DEBUG_MODE = Convert.ToBoolean(ConfigurationManager.AppSettings["DEBUG_MODE"]);
        //public const string DEBUG_LOCAL_CATALOG_FILE_ECTB_DISTRIBUTOR = ConfigurationManager.AppSettings["DEBUG_LOCAL_CATALOG_FILE_ECTB_DISTRIBUTOR"];
        //public const string DEBUG_LOCAL_CATALOG_FILE_BUILTIN_DISTRIBUTOR = ConfigurationManager.AppSettings["DEBUG_LOCAL_CATALOG_FILE_BUILTIN_DISTRIBUTOR"];



        public static string ACS_PACKAGING_SERVER = ConfigurationManager.AppSettings["ACS_PACKAGING_SERVER"];
        public static string ACS_ECTB_DISTRIBUTORID = ConfigurationManager.AppSettings["ACS_ECTB_DISTRIBUTORID"];
        public static string ACS_ECTB_SECRET_KEY = ConfigurationManager.AppSettings["ACS_ECTB_SECRET_KEY"];
        public static string ACS_ECTB_BUILTIN_DISTRIBUTORID = ConfigurationManager.AppSettings["ACS_ECTB_BUILTIN_DISTRIBUTORID"];
        public static string ACS_ECTB_BUILTIN_SECRET_KEY = ConfigurationManager.AppSettings["ACS_ECTB_BUILTIN_SECRET_KEY"];
        public static string AZURE_STORAGE_ACCT = ConfigurationManager.AppSettings["AZURE_STORAGE_ACCT"];
        public static string AZURE_BLOB_CONTAINER = ConfigurationManager.AppSettings["AZURE_BLOB_CONTAINER"];


        // local directory to stage epubs during ingestion
        public static string LOCAL_STAGING_PATH = ConfigurationManager.AppSettings["LOCAL_STAGING_PATH"];
        public static int BATCH_SIZE = Convert.ToInt32(ConfigurationManager.AppSettings["BATCH_SIZE"]);
        public static bool DELETE_FROM_FTP = Convert.ToBoolean(ConfigurationManager.AppSettings["DELETE_FROM_FTP"]);
        public static int MAX_FILE_SIZE = Convert.ToInt32(ConfigurationManager.AppSettings["MAX_FILE_SIZE"]);


        public static bool DEBUG_MODE = Convert.ToBoolean(ConfigurationManager.AppSettings["DEBUG_MODE"]);
        public static string DEBUG_LOCAL_CATALOG_FILE_ECTB_DISTRIBUTOR = ConfigurationManager.AppSettings["DEBUG_LOCAL_CATALOG_FILE_ECTB_DISTRIBUTOR"];
        public static string DEBUG_LOCAL_CATALOG_FILE_BUILTIN_DISTRIBUTOR = ConfigurationManager.AppSettings["DEBUG_LOCAL_CATALOG_FILE_BUILTIN_DISTRIBUTOR"];



        /* DEV 
       public const string serverURL = "http://hcpetgacs.eastus.cloudapp.azure.com:8080"; // Tomcat URL, no trailing slash
       public const string eCTBDistributorId = "urn:uuid:71322514-a637-4084-b54c-4319169672ad";    //dev.eCTB
       public const string eCTBSecretKey = "DrzPkXp4tbZr2HCFMRO7BDi38q0="; //dev.eCTB

       public const string builtInACS4DistributorId = "urn:uuid:00000000-0000-0000-0000-000000000001";    //builtin
       public const string builtInACS4SecretKey = "wzWKHo47W4mhc7sRXoyKDFMM1cw="; //builtin
       public const string AZURE_STORAGE_ACCT = "DefaultEndpointsProtocol=https;AccountName=bjsdev;AccountKey=SuH6hN+itIFol43cSz1EdnM/vqf3Y2OZ5/Mh1CtKY0kLvUj5Df8/NcoVJ+agfl09Kif276isBPDCawc7ZzeGPA==;";
       public const string AZURE_BLOB_CONTAINER = "ectbingestarchive";
       */
        
	    
    }
}
