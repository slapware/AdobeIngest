using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web;
using System.Net;
using System.IO;

using Adobe.Adept;
using HCPACS4;
using HCPACS4.FileIO;
using System.Threading;

namespace ACS4Ingest
{

  public partial class MainWindow : Form
  {
      private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private DistributorManager distributorManager = new DistributorManager();

      private HCPACS4.ResourceItemInfoCollection eCTB = new HCPACS4.ResourceItemInfoCollection();

    public MainWindow()
    {
      InitializeComponent();
    }

    private void MainWindow_Load(object sender, EventArgs e)
    {

    }

    private void TestWebCallButton_Click(object sender, EventArgs e)
    {
        log.Info("Fetching builtIn Distributor catalog");
        var builtInCatalogRaw = eCTB.FetchCatalog(HCPACS4.AdobeConfig.ACS_ECTB_BUILTIN_DISTRIBUTORID, HCPACS4.AdobeConfig.ACS_ECTB_BUILTIN_SECRET_KEY);
        if (!builtInCatalogRaw.IsError())
        {
            File.WriteAllText(HCPACS4.AdobeConfig.DEBUG_LOCAL_CATALOG_FILE_BUILTIN_DISTRIBUTOR, builtInCatalogRaw.ResponseBody);
        }
        log.Info("Fetching eCTB Distributor catalog");
        var eCTBCatalogRaw = eCTB.FetchCatalog(HCPACS4.AdobeConfig.ACS_ECTB_DISTRIBUTORID, HCPACS4.AdobeConfig.ACS_ECTB_SECRET_KEY);
        if (!eCTBCatalogRaw.IsError())
        {
            File.WriteAllText(HCPACS4.AdobeConfig.DEBUG_LOCAL_CATALOG_FILE_ECTB_DISTRIBUTOR, eCTBCatalogRaw.ResponseBody);
            var eCtbCatalog = HCPUtils.ParseACS4Catalog(builtInCatalogRaw.ResponseBody);
            var mapeCtbCatalog = HCPUtils.MapIsbnToACS4Resource(eCtbCatalog);
            string isbnlisteCtbFile = String.Format(@"{0}\isbnlisteCtb.txt", HCPACS4.AdobeConfig.LOCAL_STAGING_PATH);
            StringBuilder isbns = new StringBuilder();
            foreach (var i in mapeCtbCatalog)
            {
                if (HCPUtils.IsValidIsbn13(i.Key))
                    isbns.AppendLine(i.Key);
            }
            File.WriteAllText(isbnlisteCtbFile, isbns.ToString());
        }
        

        MessageBox.Show("Done");
    }

    private void btnPackageRequest_Click(object sender, EventArgs e)
    {
        
        if (HCPACS4.AdobeConfig.DELETE_FROM_FTP)
            MessageBox.Show("Beware Delete From FTP");

        IngestionJob.Run();

        MessageBox.Show("Done");

    }
    private void btnPackageRequest_Click_OLD(object sender, EventArgs e)
    {
        if (HCPACS4.AdobeConfig.DELETE_FROM_FTP)
            MessageBox.Show("Beware Delete From FTP");

        log.Info("Begin ACS4 ingestion process...");


        int numFilesDropped = 0;
        int numFilesIngested = 0;
        ACS4ApiResponse builtInCatalogRaw = null;
        Dictionary<string, ACS4CatalogResourceMapItem> mapBuiltInCatalog = null;
        IEnumerable<ACS4Catalog> builtInCatalog = null;
        ACS4ApiResponse eCtbCatalogRaw = null;
        Dictionary<string, ACS4CatalogResourceMapItem> eCtbCatalogMap = null;
        IEnumerable<ACS4Catalog> eCtbCatalog = null;
        List<List<HCPACS4.FileIO.HCPFileInfo>> allDroppedFiles = new List<List<HCPACS4.FileIO.HCPFileInfo>>();

        
        var dbRunLog = new ACS4Ingest.RunLog();
        dbRunLog.StartTimeUtc = System.DateTime.UtcNow;
        dbRunLog.BatchSize = HCPACS4.AdobeConfig.BATCH_SIZE;
        dbRunLog.Insert();
        
        // check for any files on drop folders
        var dropFolders = ACS4Ingest.ConfigFtpDropFolder.Query("SELECT * FROM ConfigFtpDropFolder WHERE Enabled = 1");
        foreach (var dropFolder in dropFolders)
        {
            try
            {
                log.Info(String.Format("List dropped epubs for {0} from {1}{2}", dropFolder.Alias, dropFolder.FtpHost, dropFolder.FtpRemotePath));
                
                var ftpDrop = new DropFolder(dropFolder.Id, dropFolder.Alias, dropFolder.FtpHost, dropFolder.FtpUser, dropFolder.FtpPassword, dropFolder.FtpRemotePath, dropFolder.Enabled);
                var droppedFiles = ftpDrop.ListFiles(".epub");
                var dbFileStats = new ACS4Ingest.DropFolderStat();
                dbFileStats.RunLogId = dbRunLog.Id;
                dbFileStats.DropFolderId = dropFolder.Id;
                dbFileStats.EpubCount = droppedFiles.Count;
                dbFileStats.Insert();

                numFilesDropped += droppedFiles.Count;
                log.Info(String.Format("{0} epubs found at {1}{2}", droppedFiles.Count, dropFolder.FtpHost, dropFolder.FtpRemotePath));
            }
            catch (Exception ex)
            {
                log.Error(String.Format("Exception listing files for {0} from {1}{2}", dropFolder.Alias, dropFolder.FtpHost, dropFolder.FtpRemotePath), ex);
            }
        }
        dbRunLog.NumFilesDropped = numFilesDropped;
        dbRunLog.Update();

        if (numFilesDropped == 0)
        {
            log.Info(String.Format("No epubs to process, exiting job."));
            return;
        }

       /** Get ACS4 catalogs **/
        try
        {
            // Get ACS4 Built In Distributor catalog to know if an asset already exists in ACS4 system
            log.Info(String.Format("Fetching ACS4 Built In Distributor catalog..."));
            builtInCatalogRaw = eCTB.FetchCatalog(HCPACS4.AdobeConfig.ACS_ECTB_BUILTIN_DISTRIBUTORID, HCPACS4.AdobeConfig.ACS_ECTB_BUILTIN_SECRET_KEY);
            if (!builtInCatalogRaw.IsError())
            {
                builtInCatalog = HCPUtils.ParseACS4Catalog(builtInCatalogRaw.ResponseBody);
                mapBuiltInCatalog = HCPUtils.MapIsbnToACS4Resource(builtInCatalog);
                log.Info(String.Format("ACS4 Built In Distributor fetched successfully with {0} records", builtInCatalog.Count()));
            }
            else
            {
                log.Error(String.Format("Error Fetching ACS4 Built In Distributor : {0} - {1}", builtInCatalogRaw.ErrorCode, builtInCatalogRaw.ErrorMessage));
            }

            // Get eCTB catalog - to know if epub has already been assigned distribution rights within eCTB distributor
            log.Info(String.Format("Fetching eCtb Distributor catalog..."));
            eCtbCatalogRaw = eCTB.FetchCatalog(HCPACS4.AdobeConfig.ACS_ECTB_DISTRIBUTORID, HCPACS4.AdobeConfig.ACS_ECTB_SECRET_KEY);
            if (!eCtbCatalogRaw.IsError())
            {
                eCtbCatalog = HCPUtils.ParseACS4Catalog(eCtbCatalogRaw.ResponseBody);
                eCtbCatalogMap = HCPUtils.MapIsbnToACS4Resource(eCtbCatalog);
                log.Info(String.Format("eCtb Distributor fetched successfully with {0} records", eCtbCatalog.Count()));
            }
            else
            {
                log.Error(String.Format("Error Fetching eCtb Distributor : {0} - {1}", eCtbCatalogRaw.ErrorCode, eCtbCatalogRaw.ErrorMessage));
            }
            dbRunLog.InitialCountBuiltInCatalog = builtInCatalog.Count();
            dbRunLog.InitialCounteCtbCatalog = eCtbCatalog.Count();
            dbRunLog.Update();

        }
        catch (Exception ex)
        {
            log.Error("Exception fetching ACS4 catalog(s)", ex);
        }

        /** Get the files and begin processing **/
        if (!builtInCatalogRaw.IsError() && !eCtbCatalogRaw.IsError() && numFilesDropped > 0)
        {
            // Download the files from FTP
            List<HCPFileInfo> fetchedFiles = new List<HCPFileInfo>();
            foreach (var dropFolder in dropFolders)
            {
                log.Info(String.Format("Downloading files from {0}{1} ...", dropFolder.FtpHost, dropFolder.FtpRemotePath));
                string savePath = String.Format(@"{0}\{1}", HCPACS4.AdobeConfig.LOCAL_STAGING_PATH.TrimEnd('\\'), dropFolder.Alias);
                var ftpDrop = new DropFolder(dropFolder.Id, dropFolder.Alias, dropFolder.FtpHost, dropFolder.FtpUser, dropFolder.FtpPassword, dropFolder.FtpRemotePath, dropFolder.Enabled);
                var files = ftpDrop.DownloadFiles(".epub", savePath, HCPACS4.AdobeConfig.BATCH_SIZE, HCPACS4.AdobeConfig.DELETE_FROM_FTP);
                log.Info(String.Format("Downloaded {0} files from {1}{2} ...", files.Count, dropFolder.FtpHost, dropFolder.FtpRemotePath));
                foreach (var f in files)
                {
                    f.DropFolderAlias = dropFolder.Alias;
                    f.DropFolderId = dropFolder.Id;
                }
                fetchedFiles.AddRange(files);
            }

            // load / analyze / package epub files
            foreach (var f in fetchedFiles)
            {
                bool existsBuiltInCatalog = false;
                bool existsEctbDistributionRights = false;
                bool galleyAttemptingToOverwriteNonGalley = false;

                Epub epub = new Epub(f.FullName, f.DropFolderAlias);
                var dbLog = new ACS4Ingest.IngestionLog();
                dbLog.RunLogId = dbRunLog.Id;
                dbLog.EpubFileName = f.FileName;
                dbLog.Isbn = epub.Isbn;
                dbLog.FileSize = epub.FileSizeBytes;
                dbLog.ArchivePath = f.FileArchivePath;
                dbLog.DropFolderId = f.DropFolderId;
                dbLog.IsbnFromFileName = epub.IsbnFromFileName;
                dbLog.IsbnFromOpf = epub.IsbnFromOpf;
                dbLog.OpfIdentifier = epub.OpfIdentifier;
                dbLog.IsCorrupt = epub.CorruptFile;
                dbLog.EnteredDateUtc = System.DateTime.UtcNow;
                dbLog.Insert();


                /** PreFlight Checks **/
                /** TODO: 
                 *      Needs some refactoring:
                 *             move to function
                 *             log to IngestionErrors table accomodate 1 to many to multiple to table
                **/
                if (epub.CorruptFile)
                {
                    log.Info(String.Format("FailedPreFlightCheck {0} : {1}", "CorruptFile", epub.FullName));
                    dbLog.Ingested = false;
                    dbLog.IsValidPreCheck = false;
                    dbLog.PreCheckError = "CorruptFile";
                    dbLog.Update();
                    continue;
                }

                if (epub.IsbnFromFileName != epub.IsbnFromOpf)
                {
                    log.Info(String.Format("FailedPreFlightCheck {0} : {1}", "IsbnMismatchFileNameOpf", epub.FullName));
                    dbLog.IsbnFromOpf = epub.IsbnFromOpf;
                    dbLog.Ingested = false;
                    dbLog.IsValidPreCheck = false;
                    dbLog.PreCheckError = "IsbnMismatchFileNameOpf";
                    dbLog.Update();
                    continue;
                }

                if (epub.InvalidDocType)
                {
                    log.Info(String.Format("FailedPreFlightCheck {0} : {1}", "InvalidDocType", epub.FullName));
                    dbLog.Ingested = false;
                    dbLog.IsValidPreCheck = false;
                    dbLog.PreCheckError = "InvalidDocType";
                    dbLog.Update();
                    continue;
                }

                // TODO: Zero in on this size restriction - 50 MB for now
                if (epub.FileSizeBytes > HCPACS4.AdobeConfig.MAX_FILE_SIZE)
                {
                    log.Info(String.Format("FailedPreFlightCheck {0} : {1}", "FileTooLarge", epub.FullName));
                    dbLog.Ingested = false;
                    dbLog.IsValidPreCheck = false;
                    dbLog.PreCheckError = "FileTooLarge";
                    dbLog.Update();
                    continue;
                }



                // lookup to see if isbn exists in Built In Catalog
                // ACS4 does not work on isbn13, it will happily ingest the same isbn several times 
                ACS4CatalogResourceMapItem mappedBuiltInCatalog;
                //TODO: using Isbn //if (mapBuiltInCatalog.TryGetValue(epub.IsbnFromOpf, out mappedBuiltInCatalog))
                if (mapBuiltInCatalog.TryGetValue(epub.Isbn, out mappedBuiltInCatalog))
                {
                    epub.ACS4Guid = mappedBuiltInCatalog.Guid;
                    existsBuiltInCatalog = true;

                    if (epub.IsGalley && !mappedBuiltInCatalog.IsGalley)
                    {
                        galleyAttemptingToOverwriteNonGalley = true;
                    }
                }

                // See if book is already assigned to eCTB Distributor
                if (eCtbCatalog.Any(i => i.resource == epub.ACS4Guid))
                    existsEctbDistributionRights = true;

                dbLog.IsValidPreCheck = true;
                dbLog.IngestType = existsBuiltInCatalog ? "update" : "add";


               
                /* Abort ingest if this file is a galley and existing is a non Galley */
                if (galleyAttemptingToOverwriteNonGalley)
                {
                    log.Info(String.Format("IngestAborted {0} : {1}", "GalleyCannotOverwriteNonGalley", epub.FullName));
                    dbLog.Ingested = false;
                    dbLog.IsValidPreCheck = false;
                    dbLog.PreCheckError = "GalleyCannotOverwriteNonGalley";
                    dbLog.Update();
                    continue;
                }



                PackageRequest package = new PackageRequest(epub);
                PackageRequestResponse packageResponse = package.SendRequest();

                log.Info(String.Format("{0} : {1} : {2}", (bool)packageResponse.Ingested ? "INGESTED" : "NOT INGESTED", epub.FullName, packageResponse.Ingested ? null  : String.Format("{0} : {1}", packageResponse.ErrorCode, packageResponse.ErrorMessage)));
                
                // assign to eCTB distributor if does not already exists
                ACS4ApiResponse rightsReqResponse = null;
                if ((bool)packageResponse.Ingested && !existsEctbDistributionRights)
                {
                    
                    // assign the book to the eCTB distributor
                    ManageDistributionRights rightsReq = new ManageDistributionRights(HCPACS4.AdobeConfig.ACS_ECTB_DISTRIBUTORID, packageResponse.ACS4Guid);
                    rightsReqResponse = rightsReq.AssignRights();
                    rightsReq = null;
                    numFilesIngested++;
                }



                dbLog.PackageHttpResponseCode = (int)packageResponse.HttpStatusCode;
                dbLog.Ingested = (bool)packageResponse.Ingested;
                dbLog.ACS4Guid = packageResponse.ACS4Guid;
                dbLog.FileSize = epub.FileSizeBytes;
                dbLog.PackageResponse = packageResponse.ResponseBody;
                dbLog.PackageErrorCode = packageResponse.IsError() ? packageResponse.ErrorCode : null;
                //TODO: Ensure we truncate to 256 dbLog.PackageErrorMessage = packageResponse.IsError() ? String.IsNullOrEmpty(packageResponse.ErrorMessage) ? null : packageResponse.ErrorMessage.Substring(0, 256) : null;
                dbLog.PackageErrorMessage = packageResponse.IsError() ? packageResponse.ErrorMessage : null;
                if (rightsReqResponse != null)
                {
                    dbLog.DistributionRightsAssigned = !rightsReqResponse.IsError();
                    dbLog.DistributionRightsErrorCode = rightsReqResponse.IsError() ? rightsReqResponse.ErrorCode : null;
                    //TODO: Ensure we truncate to 256 dbLog.DistributionRightsErrorMessage = rightsReqResponse.IsError() ? String.IsNullOrEmpty(rightsReqResponse.ErrorMessage) ? rightsReqResponse.ErrorMessage.Substring(0, 256) : null : null;
                    dbLog.DistributionRightsErrorMessage = rightsReqResponse.IsError() ? rightsReqResponse.ErrorMessage : null;
                }
                dbLog.EnteredDateUtc = System.DateTime.UtcNow;
                //dbLog.Insert();
                dbLog.Update();

                dbLog = null;
                package = null;
                packageResponse = null;
                rightsReqResponse = null;
            }

        }
        else
        {
            log.Info(String.Format("Nothing to do, exiting job."));
        }

        dbRunLog.NumFilesIngested = numFilesIngested;
        dbRunLog.EndTimeUtc = System.DateTime.UtcNow;
        dbRunLog.Update();

        log.Info(String.Format("Total files ingested {0}", numFilesIngested));
        log.Info("End ACS4 ingestion process");
        
        MessageBox.Show("Done");
    }

    private void btnDeleteAllBooks_Click(object sender, EventArgs e)
    {
        // See following article to understand the several delete requests that must be 
        // send to ACS to completely delete a book
        //http://feedback.datalogics.com/knowledgebase/articles/818175-foreign-key-constraint-error-when-trying-to-delete


        var builtInCatalogRaw = eCTB.FetchCatalog(HCPACS4.AdobeConfig.ACS_ECTB_BUILTIN_DISTRIBUTORID, HCPACS4.AdobeConfig.ACS_ECTB_BUILTIN_SECRET_KEY);
        IEnumerable<ACS4Catalog> catalog = null;
        if (!builtInCatalogRaw.IsError())
        {
            catalog = HCPUtils.ParseACS4Catalog(builtInCatalogRaw.ResponseBody);
        }

       
        // TODO: Debugging stuff, prevent bad stuff in production
        if (HCPACS4.AdobeConfig.ACS_PACKAGING_SERVER.Contains("hcpetgacs.eastus.cloudapp.azure.com"))
        {
            DialogResult dialogResult = MessageBox.Show(String.Format("About to Delete {0} Books from ACS4", catalog.Count()), "Delete", MessageBoxButtons.OKCancel);
            if (dialogResult == DialogResult.OK)
            {
                foreach (var i in catalog)
                {
                    var acs4DistributionRights = new ManageDistributionRights(HCPACS4.AdobeConfig.ACS_ECTB_DISTRIBUTORID, i.resource);
                    ACS4ApiResponse respDistributorRights = acs4DistributionRights.RemoveRights();

                    var acs4ResourceItem = new ManageResourceItem(i.resource);
                    ACS4ApiResponse resp = acs4ResourceItem.Delete();

                    var acs4ResourceKey = new ManageResourceKey(i.resource);
                    ACS4ApiResponse respResourceKey = acs4ResourceKey.Delete();

                    Console.WriteLine(String.Format("Deleted {0}", i.resource));
                }
                MessageBox.Show("Done");
            }
        }
        else
        {
            MessageBox.Show("Sorry - can't delete except against Dev environment");
        }
        

        
    }

  }
}
