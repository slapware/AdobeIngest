using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.FtpClient;
using System.Text;
using System.Threading.Tasks;

namespace HCPACS4.FileIO
{

    public class DropFolder
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public int Id { get; set; }
        public string Alias { get; set; }
        public string FtpHost { get; set; }
        public string FtpUser { get; set; }
        public string FtpPassword { get; set; }
        public string RemotePath { get; set; }
        public bool Enabled { get; set; }

        public DropFolder(int id, string alias, string ftpHost, string ftpUser, string ftpPassword, string remotePath, bool enabled)
        {
            this.Id = id;
            this.Alias = alias;
            this.FtpHost = ftpHost;
            this.FtpUser = ftpUser;
            this.FtpPassword = ftpPassword;
            this.RemotePath = remotePath;
            this.Enabled = enabled;
        }

        public List<HCPFileInfo> ListFiles(string fileExtension)
        {
            List<HCPFileInfo> files = new List<HCPFileInfo>();

            using (FtpClient client = new FtpClient())
            {
                client.Host = this.FtpHost;
                client.Credentials = new NetworkCredential(this.FtpUser, this.FtpPassword);
                client.SetWorkingDirectory(this.RemotePath);

                foreach (FtpListItem item in client.GetListing(client.GetWorkingDirectory(), FtpListOption.Modify | FtpListOption.Size)
                    .Where(item => String.Equals(Path.GetExtension(item.Name), fileExtension, StringComparison.CurrentCultureIgnoreCase) && HCPUtils.IsFileNamedByIsbn13(item.Name)))
                {
                    switch (item.Type)
                    {
                        case FtpFileSystemObjectType.Directory:
                            break;
                        case FtpFileSystemObjectType.File:
                            files.Add(new HCPFileInfo { FileName = item.Name, FullName = item.FullName, ModifiedDate = item.Modified, FileSize = item.Size });
                            break;
                        default:
                            break;

                    }
                }
            }
            return files;
        }


        public List<HCPFileInfo> DownloadFiles(string fileExtension, string localSavePath, int batchSize, bool deleteAfterDownload)
        {
            List<HCPFileInfo> files = new List<HCPFileInfo>();

            using (FtpClient client = new FtpClient())
            {
                client.Host = this.FtpHost;
                client.Credentials = new NetworkCredential(this.FtpUser, this.FtpPassword);
                client.SetWorkingDirectory(this.RemotePath);

                foreach (FtpListItem item in client.GetListing(client.GetWorkingDirectory(), FtpListOption.Modify | FtpListOption.Size)
                    .Where(item => String.Equals(Path.GetExtension(item.Name), fileExtension, StringComparison.CurrentCultureIgnoreCase) && HCPUtils.IsFileNamedByIsbn13(item.Name))
                    .Take(batchSize))
                {
                    switch (item.Type)
                    {
                        case FtpFileSystemObjectType.Directory:
                            break;
                        case FtpFileSystemObjectType.File:
                            try
                            {
                                using (var ftpStream = client.OpenRead(item.FullName))
                                {
                                    
                                    string fileSavedAs = FileUtils.SaveFile(localSavePath, item.Name, ftpStream);

                                    //byte[] data = FileUtils.ReadFully(ftpStream);
                                    //string fileSavedAs = FileUtils.SaveFile(localSavePath, item.Name, data);

                                    if (!String.IsNullOrEmpty(fileSavedAs))
                                    {
                                        // archive it to BlobStorage
                                        //string archivePath = FileUtils.SaveToAzureBlob(data, item.Name);
                                        string archivePath = FileUtils.SaveToAzureBlob(fileSavedAs);

                                        files.Add(new HCPFileInfo { FileName = item.Name, FullName = fileSavedAs, FileArchivePath = archivePath, ModifiedDate = item.Modified, FileSize = item.Size });

                                        if (deleteAfterDownload)
                                            client.DeleteFile(item.FullName);
                                    }
                                    else
                                    {
                                        log.Error(String.Format("Error saving file {0} to {1}", localSavePath));
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                log.Error(String.Format("Error downloading file from {0}{1}", this.FtpHost, item.FullName), ex);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return files;
        }
    }
}